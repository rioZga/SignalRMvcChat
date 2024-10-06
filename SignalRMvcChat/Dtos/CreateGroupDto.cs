using System.Collections.Generic;

namespace SignalRMvcChat.Dtos
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public List<string> members { get; set; }
    }
}
