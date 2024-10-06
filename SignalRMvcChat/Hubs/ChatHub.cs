using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRMvcChat.ChatHelpers;
using SignalRMvcChat.Models;
using SignalRMvcChat.Services;
using System;
using System.Threading.Tasks;

namespace SignalRMvcChat.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IChatService _chatService;
        private readonly UserManager<AppUser> _userManager;
        public static readonly ConnectionMapping<string> _connections = new();

        public ChatHub(IChatService chatService, UserManager<AppUser> userManager)
        {
            _chatService = chatService;
            _userManager = userManager;
        }
        public async Task SendMessage(string name, Message message)
        {
            foreach (var connectionId in _connections.GetConnections(name))
            {
                await Clients.Client(connectionId).ReceiveMessage(message.Text, message.TimeStamp, message.ConversationId, name);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var currentUser = await _userManager.GetUserAsync(Context.User);
            _connections.Add(currentUser.UserName, Context.ConnectionId);

            var userGroups = await _chatService.GetUserGroups(currentUser);

            foreach (var group in userGroups)
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.Identity.Name;
            _connections.Remove(name, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddToGroup(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }
    }
}
