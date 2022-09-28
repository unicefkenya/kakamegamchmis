using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class PMTWeight
    {
        [Key, Column(Order = 1)]
        public int ConstantId { get; set; }

        [Key, Column(Order = 2)]
        public int LocalityId { get; set; }

        public float Weight { get; set; }
        public SystemCodeDetail Locality { get; set; }
        public SystemCodeDetail Constant { get; set; }
    }
}