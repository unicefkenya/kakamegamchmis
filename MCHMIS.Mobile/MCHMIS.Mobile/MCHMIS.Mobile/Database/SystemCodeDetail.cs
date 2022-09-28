using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class SystemCodeDetail
    {
        public string Code { get; set; }

       

        [PrimaryKey, Unique]
        public int Id { get; set; }

        public decimal? OrderNo { get; set; }

        public int SystemCodeId { get; set; }

        public string Description => $"{OrderNo?.ToString("##")}.{Code}";
    }
}