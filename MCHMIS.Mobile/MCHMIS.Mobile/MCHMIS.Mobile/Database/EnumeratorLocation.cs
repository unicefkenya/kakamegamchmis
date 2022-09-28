using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class EnumeratorLocation
    {
        [PrimaryKey,Unique]
        public int Id { get; set; }
        public int EnumeratorId { get; set; }
        public int LocationId { get; set; }
        public bool IsActive { get; set; }
    }
}