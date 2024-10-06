using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SignalRMvcChat.ViewModels
{
    public class CreateGroupViewModel
    {
        [Required(ErrorMessage = "Group name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Select at least one user.")]
        [DisplayName("Group Members")]
        public List<string> GroupMembers { get; set; }
    }
}
