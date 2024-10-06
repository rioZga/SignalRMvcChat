using SignalRMvcChat.Models;
using System.Collections.Generic;

namespace SignalRMvcChat.ViewModels
{
    public class ConversationViewModel
    {
        public List<Conversations> Conversations { get; set; }
        public List<Groups> Groups { get; set; }
        public List<string> Users { get; set; }
    }

    public class Conversations
    {
        public Conversation Conversation { get; set; }
        public Message LastMessage { get; set; }
        public string RecepientName { get; set; }
    }

    public class Groups
    {
        public Conversation Conversation { get; set; }
        public Message LastMessage { get; set; }
        public string GroupName { get; set; }
    }
}
