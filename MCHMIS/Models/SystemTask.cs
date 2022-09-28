using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class SystemTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Parent")]
        public int? ParentId { get; set; }

        [DisplayName("Task Name")]
        public string Name { get; set; }

        public SystemTask Parent { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<SystemTask> Children { get; set; }

        public int? Order { get; set; }
    }
}