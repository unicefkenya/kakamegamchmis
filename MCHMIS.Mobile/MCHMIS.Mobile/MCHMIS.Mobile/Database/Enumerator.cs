using System;
using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class Enumerator
    {
        [PrimaryKey, Unique]
        public int Id { get; set; }
        public int EnumeratorGroupId { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string NationalIdNo { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? ActivityDate { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsLocked { get; set; }
        public int? DeactivatedBy { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {Surname}";
    }
}