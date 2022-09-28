using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class CutOff
    {
        [Key]
        public int SubLocationId { get; set; }
        public SubLocation SubLocation { get; set; }
        public decimal Value { get; set; }
       
    }

}