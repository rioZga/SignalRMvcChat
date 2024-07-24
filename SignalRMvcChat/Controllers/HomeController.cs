using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRMvcChat.Data;
using SignalRMvcChat.Models;
using SignalRMvcChat.ViewModels;
using System.Diagnostics;

namespace SignalRMvcChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public HomeController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var allMessages = _context.Messages.Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id).ToList();

            var chats = new List<ChatViewModel>();
            foreach (var u in await _context.Users.ToListAsync())
            {
                if (u == user) continue;

                var chat = new ChatViewModel()
                {
                    MyMessages = allMessages.Where(x => x.SenderId == user.Id && x.ReceiverId == u.Id).ToList(),
                    OtherMessages = allMessages.Where(x => x.SenderId == u.Id && x.ReceiverId == user.Id).ToList(),
                    ReceiverName = u.UserName
                };
                var chatMessages = new List<Message>();
                chatMessages.AddRange(chat.MyMessages);
                chatMessages.AddRange(chat.OtherMessages);

                chat.LastMessage = chatMessages.OrderByDescending(x => x.TimeStamp).FirstOrDefault();

                chats.Add(chat);
            }
            return View(chats);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}