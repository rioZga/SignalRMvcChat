using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SignalRMvcChat.Models
{
    public class AppUser : IdentityUser
    {
        public List<Conversation>? Conversations { get; set; }
        public bool? IsActice { get; set; }
        public string? LastActive { get; set; }
    }
}
