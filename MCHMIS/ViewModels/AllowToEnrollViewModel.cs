using System.ComponentModel;

namespace MCHMIS.ViewModels
{
    public class AllowToEnrollViewModel
    {
        public string Id { get; set; }
        public bool? AllowedToEnroll { get; set; }
        [DisplayName("Reason")]
        public string AllowedReason { get; set; }
        public int? ListTypeId { get; set; }
    }
}