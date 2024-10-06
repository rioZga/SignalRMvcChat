using SignalRMvcChat.Models;
using System.Collections.Generic;

namespace SignalRMvcChat.ViewModels
{
    public class ChatViewModel
    {
        public string ReceiverName { get; set; }
        public List<Message> MyMessages { get; set; }
        public List<Message> OtherMessages { get; set; }
        public Message LastMessage { get; set; }
    }
}
