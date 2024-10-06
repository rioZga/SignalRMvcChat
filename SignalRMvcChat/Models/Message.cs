using System;

namespace SignalRMvcChat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public AppUser Sender { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
