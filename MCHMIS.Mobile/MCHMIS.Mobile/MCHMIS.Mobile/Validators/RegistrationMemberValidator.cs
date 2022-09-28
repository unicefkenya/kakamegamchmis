using System;
using MCHMIS.Mobile.Database;
using FluentValidation;
using MCHMIS.Mobile.ViewModels;

namespace MCHMIS.Mobile.Validators
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty().Matches(@"^[0-9]{1,8}$").WithMessage("National ID No. is required.");
            RuleFor(x => x.Pin).NotEmpty().Matches(@"^[0-9]{1,6}$").WithMessage("Pin is required.");
        }
    }
    public class RegistrationMemberValidator : AbstractValidator<RegistrationMember>
    {
        public RegistrationMemberValidator()
        {
            RuleFor(x => x.FirstName).MaximumLength(20).Matches(@"^[A-Za-z_'-- ]{1,20}$").WithMessage("Check  First Name");
            RuleFor(x => x.Surname).Matches(@"^[A-Za-z_'-- ]{1,20}$").WithMessage("Check  Surname");
            RuleFor(x => x.MiddleName).Matches(@"^[A-Za-z_'-- ]{1,20}$").WithMessage("Check  Middle Name").Unless(x => string.IsNullOrEmpty(x.MiddleName));
            RuleFor(x => x.PhoneNumber).Matches(@"^[0-9+-- ]{1,10}$").WithMessage("Check 3.01(b)").MaximumLength(10).WithMessage("Check 3.01(b) ").Unless(x => string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.RelationshipId).NotNull().GreaterThan(0).WithMessage("3.03 Response is required");
            RuleFor(x => x.SexId).NotNull().GreaterThan(0).WithMessage("3.04 Response is required");
            RuleFor(x => x.MaritalStatusId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.06");

           
            RuleFor(x => x.FatherAliveStatusId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.08");
            RuleFor(x => x.MotherAliveStatusId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.09");
            RuleFor(x => x.ChronicIllnessStatusId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.10 Response");
            RuleFor(x => x.DisabilityTypeId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.11 Response");
            RuleFor(x => x.DisabilityCareStatusId).NotNull() .GreaterThanOrEqualTo(0).WithMessage("Check 3.12 Response").Unless(x=>x.DisabilityCareStatusId==null||x.DisabilityCareStatusId==0);
            RuleFor(x => x.CareGiverId).NotNull().WithMessage("Check 3.13 Response").Unless(x=> string.IsNullOrEmpty(x.CareGiverId));
            RuleFor(x => x.LearningStatusId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.14 Response").Unless(x=>x.DateOfBirth>DateTime.Today.AddYears(-3));
            // RuleFor(x => x.EducationLevelId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.15 Response").Unless(x => x.DateOfBirth > DateTime.Today.AddYears(-3));
            RuleFor(x => x.WorkTypeId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.16 Response").Unless(x => x.DateOfBirth > DateTime.Today.AddYears(-3)); ;
            RuleFor(x => x.FormalJobNgoId).NotNull().GreaterThanOrEqualTo(0).WithMessage("Check 3.17 Response").Unless(x => x.DateOfBirth > DateTime.Today.AddYears(-3)); ;
        }
    }
}