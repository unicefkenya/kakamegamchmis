using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class SystemCode
    {
        public string Code { get; set; }

        public string Description { get; set; }

        [PrimaryKey, Unique]
        public int Id { get; set; }

        public bool IsUserMaintained { get; set; }


    }
}