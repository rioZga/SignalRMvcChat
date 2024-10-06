using System.Collections.Generic;

namespace SignalRMvcChat.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<Message> Messages { get; set; }
        public List<AppUser> Users { get; set; }
    }
}
