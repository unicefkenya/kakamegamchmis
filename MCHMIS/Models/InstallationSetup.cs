using System.ComponentModel;

namespace MCHMIS.Models
{
    public class InstallationSetup
    {
        public int Id { get; set; }

        [DisplayName("Health Facility")]
        public int HealthFacilityId { get; set; }
        public HealthFacility HealthFacility { get; set; }
    }
}