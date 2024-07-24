using Microsoft.AspNetCore.SignalR;
using SignalRMvcChat.ChatHelpers;
using SignalRMvcChat.Models;

namespace SignalRMvcChat.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public static readonly ConnectionMapping<string> _connections = new();
        public async Task SendMessage(string name, Message message)
        {
            foreach (var connectionId in _connections.GetConnections(name))
            {
                await Clients.Client(connectionId).ReceiveMessage(message.Text, message.TimeStamp.ToShortTimeString());
            }
        }

        public override async Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.Identity.Name;
            _connections.Remove(name, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
