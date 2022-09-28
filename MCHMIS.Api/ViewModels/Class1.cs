namespace MCHMIS.Api.ViewModels
{
    public class CodeVm
    {
        public string Code { get; set; }

        public decimal? OrderNo { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }
    }

    public class RegionVm
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class SubLocationVm : RegionVm
    {
        public int LocationId { get; set; }
    }

    public class WardVm : RegionVm
    {
        public string Code { get; set; }
    }

    public class VillageVm : RegionVm
    {
        public string Code { get; set; }
    }

    public class CommunityAreaVm : RegionVm
    {
        public string Code { get; set; }
    }

    public class SystemCodeDetailVm : CodeVm
    {
        public int SystemCodeId { get; set; }
    }

    public class SystemCodeVm : CodeVm
    {
    }
}