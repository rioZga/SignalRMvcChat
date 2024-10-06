using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRMvcChat.Data;
using SignalRMvcChat.Models;
using SignalRMvcChat.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SignalRMvcChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public HomeController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var conversations = _context.Conversations
                .AsNoTracking()
                .Include(c => c.Messages)
                .Include(c => c.Users)
                .Where(c => c.Users.Contains(user));

            List<Conversations> convs = new();
            List<Groups> groups = new();
            foreach (var conversation in conversations)
            {
                if (conversation.Type == ConversationType.Private)
                {
                    Conversations conv = new()
                    {
                        Conversation = conversation,
                        LastMessage = conversation.Messages.OrderByDescending(m => m.TimeStamp).FirstOrDefault()!,
                        RecepientName = conversation.Users.Where(u => u.Id != user.Id).FirstOrDefault().UserName,
                    };
                    convs.Add(conv);
                }
                else
                {
                    Groups gr = new()
                    {
                        GroupName = conversation.Name,
                        Conversation = conversation,
                        LastMessage = conversation.Messages.OrderByDescending(m => m.TimeStamp).FirstOrDefault()!,
                    };
                    groups.Add(gr);
                }
                
            }

            var ConversationVM = new ConversationViewModel
            {
                Conversations = convs,
                Groups = groups,
                Users = _context.Users.AsNoTracking().Where(u => u.UserName != user.UserName).Select(u => u.UserName).ToList(),
            };

            return View(ConversationVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}