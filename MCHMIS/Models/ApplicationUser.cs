using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MCHMIS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string IdNumber { get; set; }
        public string FullName => FirstName + " " + MiddleName + " " + Surname;

        public string Name => FirstName + " " + Surname;

        public string DisplayName => FirstName + " " + Surname;

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [DisplayName("Date Created"), DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }

        [DisplayName("Created By")]
        public ApplicationUser CreatedBy { get; set; }

        public string MacAddress { get; set; }
        public bool IsLoggedIn { get; set; }

        public int? SubCountyId { get; set; }
        public SubCounty SubCounty { get; set; }
    }
}