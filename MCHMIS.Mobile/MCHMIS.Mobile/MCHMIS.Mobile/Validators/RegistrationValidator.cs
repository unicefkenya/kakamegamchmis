using MCHMIS.Mobile.Database;
using FluentValidation;

namespace MCHMIS.Mobile.Validators
{

    public class RegistrationValidator : AbstractValidator<Registration>
    {
        public RegistrationValidator()
        {
            //RuleFor(x => x.WardId).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Ward is Required");
           // RuleFor(x => x.SubLocationId).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Sub Location is Required");
            //RuleFor(x => x.Village).NotEmpty().Matches(@"^[A-Za-z0-9_'-- ]{1,100}$").WithMessage("Check Village Name");
            //RuleFor(x => x.ResidenceDurationYears).NotNull().GreaterThan(-1).LessThanOrEqualTo(100).WithMessage("Select the Years");
           // RuleFor(x => x.ResidenceDurationMonths).NotNull().GreaterThan(-1).LessThanOrEqualTo(12).WithMessage("Select the Months");
           // RuleFor(x => x.NearestReligiousBuilding).NotEmpty().Matches(@"^[A-Za-z0-9_'-- ]{1,100}$").WithMessage("Check Nearest Religious Building");
          //  RuleFor(x => x.NearestSchool).NotEmpty().Matches(@"^[A-Za-z0-9_'-- ]{1,100}$").WithMessage("Check Nearest School"); ;

            RuleFor(x => x.HouseholdMembers).NotNull().GreaterThan(0).WithMessage("Check Household Members");

            RuleFor(x => x.IsTelevisionId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.11");
            RuleFor(x => x.IsMotorcycleId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.12");
            RuleFor(x => x.IsTukTukId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.13");
            RuleFor(x => x.IsRefrigeratorId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.14");
            RuleFor(x => x.IsCarId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.15");
            RuleFor(x => x.IsMobilePhoneId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.16");
            RuleFor(x => x.IsBicycleId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.17");

            RuleFor(x => x.ExoticCattle).NotNull().GreaterThan(-1).WithMessage("Check 2.18");
            RuleFor(x => x.IndigenousCattle).NotNull().GreaterThan(-1).WithMessage("Check 2.19");
            RuleFor(x => x.Sheep).NotNull().GreaterThan(-1).WithMessage("Check 2.20");
            RuleFor(x => x.Goats).NotNull().GreaterThan(-1).WithMessage("Check 2.21");
            RuleFor(x => x.Camels).NotNull().GreaterThan(-1).WithMessage("Check 2.22");
            RuleFor(x => x.Donkeys).NotNull().GreaterThan(-1).WithMessage("Check 2.23");
            RuleFor(x => x.Pigs).NotNull().GreaterThan(-1).WithMessage("Check 2.24");
            RuleFor(x => x.Chicken).NotNull().GreaterThan(-1).WithMessage("Check 2.25");


            RuleFor(x => x.LiveBirths).NotNull().GreaterThan(-1).WithMessage("Check 2.26");
            RuleFor(x => x.Deaths).NotNull().GreaterThan(-1).WithMessage("Check 2.27");

            RuleFor(x => x.HouseHoldConditionId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.28");
            RuleFor(x => x.IsSkippedMealId).NotNull().NotEmpty().GreaterThan(-1).WithMessage("Check 2.29");
            RuleFor(x => x.NsnpProgrammesId).NotNull().GreaterThan(-1).WithMessage("Check 2.30");

            RuleFor(x => x.IsReceivingSocialId).NotNull().GreaterThan(-1).WithMessage("Check 2.31");

            RuleFor(x => x.OtherProgrammeNames).NotEmpty().Matches(@"^[A-Za-z0-9_'--, ]{1,100}$").WithMessage("Check 2.32").Unless(x => string.IsNullOrEmpty(x.OtherProgrammeNames));
            RuleFor(x => x.BenefitTypeId).NotNull().GreaterThan(-1).WithMessage("Check 2.33");
             
            RuleFor(x => x.LastReceiptAmount).NotNull().WithMessage("Check 2.34").Unless(x=>x.LastReceiptAmount == 0);
            RuleFor(x => x.InKindBenefitId).NotEmpty().Matches(@"^[A-Za-z0-9_'--, ]{1,100}$").WithMessage("Check 2.35").Unless(x =>  string.IsNullOrEmpty(x.InKindBenefitId));

        }
    }
}