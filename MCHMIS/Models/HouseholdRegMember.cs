using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class HouseholdRegMember
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        public string Surname { get; set; }

        [DisplayName("Identification Document")]
        public int? IdentificationFormId { get; set; }

        public SystemCodeDetail IdentificationForm { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Relationship to the head of this household")]
        public int? RelationshipId { get; set; }

        [DisplayName("Gender")]
        public int? GenderId { get; set; }

        [DisplayName("Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DOB { get; set; }

        public int? Age
        {
            get
            {
                int? age=null;
                if (DOB != null)
                {
                    DateTime now = DateTime.Today;
                     age = now.Year - ((DateTime)DOB).Year;
                    if (DOB > now.AddYears((int) -age)) age--;
                }
               
                return age;
            }
        }

        [DisplayName("Marital Status")]
        public int? MaritalStatusId { get; set; }

        [DisplayName("Does Spouse live in this household?")]
        public int? SpouseInHouseholdId { get; set; }

        public SystemCodeDetail SpouseInHousehold { get; set; }

        [DisplayName("Is father alive?")]
        public int? FatherAliveId { get; set; }

        [DisplayName("Is mother alive?")]
        public int? MotherAliveId { get; set; }

        [DisplayName("Suffers from chronic illness")]
        public int? ChronicIllnessId { get; set; }

        public SystemCodeDetail ChronicIllness { get; set; }

        [DisplayName("Main Care Giver")]
        public string DisabilityCaregiverId { get; set; }

        [DisplayName("Does Disability require 24-hour care?")]
        public int? DisabilityRequires24HrCareId { get; set; }

        public SystemCodeDetail DisabilityRequires24HrCare { get; set; }

        [DisplayName("Main Care Giver")]
        public HouseholdRegMember DisabilityCaregiver { get; set; }

        public bool? ExternalMember { get; set; }

        public int? MainCaregiverId { get; set; }

        [DisplayName("Learning Institution attendance status")]
        public int? EducationAttendanceId { get; set; }

        [DisplayName("Highest Std/Form/Level reached by the member?")]
        public int? EducationLevelId { get; set; }

        [DisplayName("School Type")]
        public int? SchoolTypeId { get; set; }

        public SystemCodeDetail SchoolType { get; set; }

        [DisplayName("What was the member doing during the last seven days?")]
        public int? OccupationTypeId { get; set; }

        [DisplayName("Does member work on a formal job, teaching, public sector, NGO/FBO?")]
        public int? FormalJobTypeId { get; set; }

        [DisplayName("HIV Status")]
        public string SupportStatusId { get; set; }

        [DisplayName("MUAC")]
        public decimal? MUAC { get; set; }

        public DateTime CreateOn { get; set; }
        public HouseholdReg Household { get; set; }

        public SystemCodeDetail Relationship { get; set; }
        public SystemCodeDetail Gender { get; set; }
        public SystemCodeDetail MaritalStatus { get; set; }
        public SystemCodeDetail OrphanhoodType { get; set; }
        public SystemCodeDetail FatherAlive { get; set; }
        public SystemCodeDetail MotherAlive { get; set; }
        public SystemCodeDetail IllnessType { get; set; }
        public SystemCodeDetail DisabilityType { get; set; }

        public SystemCodeDetail EducationAttendance { get; set; }
        public SystemCodeDetail EducationLevel { get; set; }
        public SystemCodeDetail OccupationType { get; set; }
        public SystemCodeDetail FormalJobType { get; set; }
        public ICollection<HouseholdRegMemberDisability> HouseholdRegMemberDisabilities { get; set; }

        [DisplayName("Name")]
        public string FullName => FirstName + " " + MiddleName + " " + Surname;
    }
}