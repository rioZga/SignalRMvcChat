using SignalRMvcChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRMvcChat.Services
{
    public interface IChatService
    {
        Conversation? GetConversation(int conversationId);
        Conversation? GetConversation(AppUser currentUser, AppUser username);
        Task<Conversation> InitiateConversation(AppUser currentUser, AppUser receiver);
        Task<Conversation> InitiateGroupConversation(string name, List<AppUser> users);
        Task<Message> AddMessage(AppUser currentUser, int conversationId, string text);
        Task<List<Message>> GetConversationMessages(int conversationId);
        Task<List<Conversation>> GetUserGroups(AppUser currentUser);
    }
}
