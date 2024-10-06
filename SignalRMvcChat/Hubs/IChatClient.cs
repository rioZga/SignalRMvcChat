using SignalRMvcChat.Models;
using System;
using System.Threading.Tasks;

namespace SignalRMvcChat.Hubs
{
    public interface IChatClient
    {
        Task SendMessage(string username, Message message);
        Task ReceiveMessage(string message, DateTime timeStamp, int conversationId, string username);
        Task AddToGroup(int groupId);
        Task SendGroupMessage(string message, DateTime timeStamp, int ConversationId, string username);
    }
}
