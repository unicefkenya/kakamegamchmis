using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class RegistrationMemberDisability
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }
        public string RegistrationMemberId { get; set; }
        public int DisabilityId { get; set; }

        public int RegistrationId { get; set; }
    }
}