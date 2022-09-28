using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class Ward
    {
        public string Code { get; set; }
        public string Name { get; set; }
        [PrimaryKey, Unique]
        public int Id { get; set; }
    }
}
