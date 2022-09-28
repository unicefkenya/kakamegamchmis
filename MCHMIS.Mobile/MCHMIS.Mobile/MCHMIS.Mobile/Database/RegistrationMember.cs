using System;
using SQLite;

namespace MCHMIS.Mobile.Database
{
    public class RegistrationMember
    {
        [PrimaryKey, Unique, AutoIncrement]
        public int? Id { get; set; }
        public string MemberId { get; set; }
        public string CareGiverId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthDate { get; set; }       
        public string FirstName { get; set; }
        // public int HasIdNumberId { get; set; }
        public string IdentificationNumber { get; set; }
        public string PhoneNumber { get; set; }
        public int? SpouseInHouseholdId { get; set; }
        public string SpouseId { get; set; }
        public string Surname { get; set; }
        public int? ChronicIllnessStatusId { get; set; }

        public int? DisabilityCareStatusId { get; set; }

        public int? DisabilityTypeId { get; set; }

        public int? EducationLevelId { get; set; }

        public int? FatherAliveStatusId { get; set; }
        public int? SchoolTypeId { get; set; }

        public int? FormalJobNgoId { get; set; }

        public int? IdentificationDocumentTypeId { get; set; }

        public int? LearningStatusId { get; set; }

        public int? MaritalStatusId { get; set; }

        public int? MotherAliveStatusId { get; set; }

        public int? RelationshipId { get; set; }

        public int? SexId { get; set; }

        //  public int? WorkLevelId { get; set; }

        public int? WorkTypeId { get; set; }

        public string MiddleName { get; set; }

        public int RegistrationId { get; set; }

        //   public int? SpouseLineNumber { get; set; }

        public string AddTime { get; set; }

        //public string DateOfBirthDate { get; set; } 
        public string FullName => $"{Surname}, {FirstName} {MiddleName}";

        public bool IPRSed { get; set; } = false;
    }
}