using System.ComponentModel;

namespace MCHMIS.Models
{
    public class HealthFacility
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Sub County")]
        public int SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }
    }
}