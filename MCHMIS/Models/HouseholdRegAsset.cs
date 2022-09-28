using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class HouseholdRegAsset
    {
        [Key, Column(Order = 1)]
        public string HouseholdId { get; set; }

        [Key, Column(Order = 2)]
        public int AssetId { get; set; }

        public int AssetTypeId { get; set; }
        public SystemCodeDetail AssetType { get; set; }

        public bool? HasItem { get; set; }

        public int? ItemCount { get; set; }

        public SystemCodeDetail Asset { get; set; }
        public HouseholdReg Household { get; set; }
    }
}