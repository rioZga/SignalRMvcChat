using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRMvcChat.Data;
using SignalRMvcChat.Hubs;
using SignalRMvcChat.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SignalRMvcChat.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public ChatController(UserManager<IdentityUser> userManager, AppDbContext context, IHubContext<ChatHub, IChatClient> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> SendMessage(string username, string text)
        {
            var user = await _userManager.GetUserAsync(User);
            var receiver = _context.Users.FirstOrDefault(u => u.UserName == username);

            Message message = new Message
            {
                SenderId = user.Id,
                ReceiverId = receiver.Id,
                Text = text,
                TimeStamp = DateTime.Now,
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            var receiverConnections = ChatHub._connections.GetConnections(username);
            foreach (var connectionId in receiverConnections)
            {
                await _hubContext.Clients.Client(connectionId).ReceiveMessage(message.Text, message.TimeStamp.ToShortTimeString());
            }

            return Ok();
        }
    }
}
