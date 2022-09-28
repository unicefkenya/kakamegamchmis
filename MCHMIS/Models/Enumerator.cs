using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class Enumerator
    {
        public int Id { get; set; }
        [DisplayName("Enumerator Group")]
        public int EnumeratorGroupId { get; set; }
        public SystemCodeDetail EnumeratorGroup { get; set; }
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

        [DisplayName("Village")]
        public int? VillageId { get; set; }
        public Village Village { get; set; }
    
    }
    public class EnumeratorDevice
    {
        public int Id { get; set; }
        [Display(Name = "Device ID")]
        public string DeviceId { get; set; }
        [Display(Name = "Model")]
        public string DeviceModel { get; set; }
        [Display(Name = "Manufacturer")]
        public string DeviceManufacturer { get; set; }
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }
        public string Version { get; set; }
        [Display(Name = "Version No.")]
        public string VersionNumber { get; set; }

        public string Platform { get; set; }
        [Display(Name = "Device Type")]
        public string Idiom { get; set; }
        [Display(Name = "Device")]
        public bool IsDevice { get; set; }
        [Display(Name = "Enumerator")]
        public int EnumeratorId { get; set; }
    }

}