using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class RoleProfile
    {
        [Key, Column(Order = 1)]
        public int TaskId { get; set; }
        [Key, Column(Order = 2)]
        public string RoleId { get; set; }

        [ForeignKey("TaskId")]
        public SystemTask SystemTask { get; set; }
    }
}