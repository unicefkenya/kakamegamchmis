using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class Village
    {
        public string Code { get; set; }

        [PrimaryKey, Unique]
        public int Id { get; set; }

        public int WardId { get; set; }
        public string Name { get; set; }
    }

    public class CommunityArea
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}