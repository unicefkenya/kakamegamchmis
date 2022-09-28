using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class PMTCutOff
    {
        [Key, Column(Order = 1)]
        public int PMTModelId { get; set; }

        //[Key, Column(Order = 2)]
        //public int LocalityId { get; set; }

        // public int PovertyLevelId { get; set; }

        public float USD { get; set; }

        public float KES { get; set; }

        public float LogScale { get; set; }

        // public SystemCodeDetail Locality { get; set; }
        //public SystemCodeDetail PMTModel { get; set; }
        //public SystemCodeDetail PovertyLevel { get; set; }
    }
}