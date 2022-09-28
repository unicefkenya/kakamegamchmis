namespace MCHMIS.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool Over18 { get; set; }
        public int IdentificationFormId { get; set; }
        public int WardId { get; set; }
        public string Institution { get; set; }
        public string Landmark { get; set; }
        public string Phone { get; set; }
        public string GuardianPhone { get; set; }
        public byte[] FingerPrint { get; set; }
        public bool VerifyingFingerPrint { get; set; }
        public string DisplayName => FirstName + " " + MiddleName + " " + LastName;
    }
}