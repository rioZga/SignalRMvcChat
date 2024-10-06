using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRMvcChat.Data;
using SignalRMvcChat.Dtos;
using SignalRMvcChat.Hubs;
using SignalRMvcChat.Models;
using SignalRMvcChat.Services;
using SignalRMvcChat.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRMvcChat.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;
        private readonly IChatService _chatService;

        public ChatController(UserManager<AppUser> userManager, IHubContext<ChatHub, IChatClient> hubContext, IChatService chatService, AppDbContext context)
        {
            _userManager = userManager;
            _hubContext = hubContext;
            _chatService = chatService;
            _context = context;
        }

        public async Task<IActionResult> SendMessage(MessageDto messageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentUser = await _userManager.GetUserAsync(User);

            Conversation? conversation;
            if (!messageDto.conversationId.HasValue)
            {
                var receiver = await _userManager.FindByNameAsync(messageDto.username);
                if (receiver == null)
                    return BadRequest("No user with this username");

                conversation = _chatService.GetConversation(currentUser, receiver);
                conversation ??= await _chatService.InitiateConversation(currentUser, receiver);
            }
            else
                conversation = _chatService.GetConversation(messageDto.conversationId.Value);

            if (conversation == null)
                return NotFound();

            var message = await _chatService.AddMessage(currentUser!, conversation.Id, messageDto.text);

            if (conversation.Type == ConversationType.Private)
            {
                var receiverConnections = ChatHub._connections.GetConnections(messageDto.username);
                foreach (var connectionId in receiverConnections)
                {
                    await _hubContext.Clients.Client(connectionId)
                        .ReceiveMessage(message.Text, message.TimeStamp, conversation.Id, message.SenderName);
                }
            }
            else
            {
                await _hubContext.Clients
                .Group(messageDto.conversationId.ToString()!)
                .ReceiveMessage(message.Text, message.TimeStamp, message.ConversationId, message.SenderName);
            }


            return Ok(new
            {
                Username = message.SenderName,
                ConversationId = conversation.Id,
                Text = message.Text,
                Time = message.TimeStamp
            });
        }

        public async Task<IActionResult> GetConversation([FromQuery] string username,
                                                     [FromQuery] int? conversationId)
        {
            if (conversationId == null && username == null)
                return BadRequest();

            if (conversationId == null)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var receiver = await _userManager.FindByNameAsync(username!);

                if (currentUser == null || receiver == null)
                {
                    return NotFound();
                }

                var conversation = _chatService.GetConversation(currentUser, receiver);
                if (conversation == null)
                {
                    return NotFound();
                }

                return Ok(conversation);
            }
            else
            {
                var conversation = _chatService.GetConversation(conversationId.Value);
                if (conversation == null)
                {
                    return NotFound();
                }

                return Ok(conversation);
            }

        }

        public async Task<IActionResult> CreateGroup()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Users = _context.Users.Where(u => u.UserName != user.UserName).Select(u => u.UserName).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupViewModel createGroupVM)
        {
            if (!ModelState.IsValid)
                return View();

            var currentUser = await _userManager.GetUserAsync(User);

            List<AppUser> users = new List<AppUser>
            {
                currentUser!,
            };
            foreach (var username in createGroupVM.GroupMembers)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    continue;
                users.Add(user);
            }

            await _chatService.InitiateGroupConversation(createGroupVM.Name, users);
            return RedirectToAction("Index", "Home");
        }
    }
}
