using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class RegistrationProgramme
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public int ProgrammeId { get; set; }
     

    }
}