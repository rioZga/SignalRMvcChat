using SignalRMvcChat.Models;

namespace SignalRMvcChat.Hubs
{
    public interface IChatClient
    {
        Task SendMessage(string username, Message message);
        Task ReceiveMessage(string message, string time);
    }
}
