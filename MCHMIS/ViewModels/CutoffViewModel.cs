using System.Collections.Generic;
using System.ComponentModel;
using MCHMIS.Models;

namespace MCHMIS.ViewModels
{
    public class CutoffsViewModel
    {
        public LocationCutoff LocationCutoff { get; set; }
        public ICollection<LocationCutoff> LocationCutoffs { get; set; }
        public ICollection<SubLocation> SubLocations { get; set; }
        public List<int> Ids { get; set; }
        public List<int> Values { get; set; }
        public List<IdValue> IdValues { get; set; }
    }

    public class LocationCutoff
    {
        public int Id { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string SubLocation { get; set; }
        public decimal? Value { get; set; }

    }

    public class IdValue
    {
        public int Id { get; set; }
        public decimal? Value { get; set; }
        public decimal? OldValue { get; set; }
    }
    public class CutOffEditViewModel
    {
        [DisplayName("Sub Location")]
        public int SubLocationId { get; set; }
        public decimal? Value { get; set; }
        public string SubLocationName { get; set; }
        public bool IsNew { get; set; }
    }
}