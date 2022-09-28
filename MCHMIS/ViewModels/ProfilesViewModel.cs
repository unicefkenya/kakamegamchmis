using System.Collections.Generic;
using System.ComponentModel;
using MCHMIS.Models;

namespace MCHMIS.ViewModels
{
    public class ProfilesViewModel
    {
        public ICollection<SystemTask> SystemTasks { get; set; }
        public int[] Ids { get; set; }
        [DisplayName("Role")]
        public string RoleId { get; set; }
        public ICollection<int> RoleProfileIds { get; set; }
    }
}