using Microsoft.AspNetCore.Identity;

namespace SignalRMvcChat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        
        public string SenderId { get; set; }
        public IdentityUser Sender { get; set; }

        public string ReceiverId { get; set; }
        public IdentityUser Receiver { get; set; }
    }
}
