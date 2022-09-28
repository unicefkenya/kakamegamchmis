using FluentValidation;
using FormsToolkit;
using MCHMIS.Mobile.Converters;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Models;
using MCHMIS.Mobile.Pages;
using MCHMIS.Mobile.Validators;
using MvvmHelpers;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class RegistrationEditViewModel : LocalBaseViewModel
    {
        public ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>> _loadedProgrammes = new ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>>();

        public ObservableRangeCollection<SystemCodeDetail> BenefitTypes = new ObservableRangeCollection<SystemCodeDetail>();
        public ObservableRangeCollection<SystemCodeDetail> FamilyBenefitTypes = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> CookingFuels = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> DwellingRisks = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> FloorConstructionMaterials = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> HouseholdConditions = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> InterviewStatus = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsReceivingSocials = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsReceivingFamilys = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> LightingFuels = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> NsnpBenefits = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> OtherProgrammes = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> RoofConstructionMaterials = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> SkippedMeals = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> TenureStatuses = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> WallConstructionMaterials = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> WasteDisposals = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> WaterSources = new ObservableRangeCollection<SystemCodeDetail>();

        private readonly IValidator _validator;

        private ICommand _editHouseholdCommand;

        private ICommand _saveHouseHoldCommand;

        private SystemCodeDetail _selectedBenefitType;

        private SystemCodeDetail _selectedFamilyBenefitType;

        private SystemCodeDetail _selectedCookingFuel;

        private SystemCodeDetail _selectedDwellingRisk;

        private SystemCodeDetail _selectedFloorConstructionMaterial;

        private SystemCodeDetail _selectedHouseholdConditions;

        private SystemCodeDetail _selectedInterviewStatus;

        private SystemCodeDetail _selectedIsBicycle;

        private SystemCodeDetail _selectedIsCar;

        private SystemCodeDetail _selectedIsMobilePhone;

        private SystemCodeDetail _selectedIsMotorcycle;

        private SystemCodeDetail _selectedIsReceivingSocial;
        private SystemCodeDetail _selectedIsReceivingFamily;

        private SystemCodeDetail _selectedIsRefrigerator;

        private SystemCodeDetail _selectedIsTelevision;

        private SystemCodeDetail _selectedIsTukTuk;

        private SystemCodeDetail _selectedLightingFuel;

        private SystemCodeDetail _selectedNsnpBenefit;

        private ObservableCollection<SystemCodeDetail> _selectedNsnpProgrammes;

        private SystemCodeDetail _selectedOtherProgramme;

        private SystemCodeDetail _selectedOwnershipOption;

        private SystemCodeDetail _selectedRoofConstructionMaterial;

        private SystemCodeDetail _selectedSkippedMeal;

        private SystemCodeDetail _selectedTenureStatus;

        private SystemCodeDetail _selectedWallConstructionMaterial;

        private SystemCodeDetail _selectedWasteDisposal;

        private SystemCodeDetail _selectedWaterSource;
        private bool isFamilyInKind;

        private bool isNotFamilyInKind;

        private bool isInKind;

        private bool isNotInKind;

        private bool isNotRecievingSocial;
        private bool isNotRecievingFamily;

        private bool isOtherProgramme;

        private bool isRecievingSocialItem;
        private bool isRecievingFamilyItem;

        private RegistrationMember selectedMember;

        public RegistrationEditViewModel(INavigation navigation, int id) : base(navigation)
        {
            var data = App.Database.GetTableRow("Registration", "Id", id.ToString());
            Registration = (Registration)data;

            //  Registration.CommunityArea =
            //     (CommunityArea)App.Database.GetTableRow("CommunityArea", "id", Registration.CommunityAreaId.ToString());

            Registration.Village =
           (Village)App.Database.GetTableRow("Village", "id", Registration.VillageId.ToString());

            Registration.Ward =
                (Ward)App.Database.GetTableRow("Ward", "id", Registration.WardId.ToString());
            HouseholdMembers.AddRange(GetHouseholdMembers(id));

            Id = id;

            LoadedTenureStatuses.AddRange(App.Database.SystemCodeDetailsGetByCode("Tenure Status"));
            LoadedRoofConstructionMaterials.AddRange(App.Database.SystemCodeDetailsGetByCode("Roof Material"));
            LoadedWallConstructionMaterials.AddRange(App.Database.SystemCodeDetailsGetByCode("Wall Material"));
            LoadedFloorConstructionMaterials.AddRange(App.Database.SystemCodeDetailsGetByCode("Floor Material"));
            LoadedDwellingRisks.AddRange(App.Database.SystemCodeDetailsGetByCode("Dwelling Unit Risk"));
            LoadedWaterSources.AddRange(App.Database.SystemCodeDetailsGetByCode("Water Source"));
            LoadedWasteDisposals.AddRange(App.Database.SystemCodeDetailsGetByCode("Toilet Type"));
            LoadedCookingFuels.AddRange(App.Database.SystemCodeDetailsGetByCode("Cooking Fuel"));
            LoadedLightingFuels.AddRange(App.Database.SystemCodeDetailsGetByCode("Lighting Source"));
            LoadedHouseholdConditions.AddRange(App.Database.SystemCodeDetailsGetByCode("Household Conditions"));
            var otherP = App.Database.SystemCodeDetailsGetByCode("Other SP Programme").ToList();
            LoadedNsnpBenefits.AddRange(otherP);

            var benefitTypes = App.Database.SystemCodeDetailsGetByCode("SP Benefit Type");

            LoadedBenefitTypes.AddRange(benefitTypes);
            LoadedFamilyBenefitTypes.AddRange(benefitTypes);

            LoadedInterviewStatus.AddRange(App.Database.SystemCodeDetailsGetByCode("Interview Result"));

            var booleanOptions = App.Database.SystemCodeDetailsGetByCode("Household Option").Where(X => X.Code != "Don't Know").ToList();

            LoadedOwnershipOptions.AddRange(booleanOptions);
            LoadedIsBicycles.AddRange(booleanOptions);
            LoadedIsCars.AddRange(booleanOptions);
            LoadedIsMobilePhones.AddRange(booleanOptions);
            LoadedIsMotorcycles.AddRange(booleanOptions);
            LoadedIsTukTuks.AddRange(booleanOptions);
            LoadedIsRefrigerators.AddRange(booleanOptions);
            LoadedIsTelevisions.AddRange(booleanOptions);
            LoadedSkippedMeals.AddRange(booleanOptions);
            LoadedOtherProgrammes.AddRange(booleanOptions);
            LoadedIsReceivingSocials.AddRange(booleanOptions);
            LoadedIsReceivingFamilys.AddRange(booleanOptions);

            var progs = App.Database.GetTableRows<RegistrationProgramme>("RegistrationProgramme", "RegistrationId", Registration.Id.ToString());

            var list = new List<SelectableItemWrapper<SystemCodeDetail>>();

            if (!progs.Any())
            {
                foreach (var item in otherP)
                {
                    list.Add(new SelectableItemWrapper<SystemCodeDetail> { Item = item, IsSelected = false });
                }
            }
            else
            {
                foreach (var item in otherP)
                {
                    list.Add(new SelectableItemWrapper<SystemCodeDetail>
                    { Item = item, IsSelected = progs.Any(x => x.ProgrammeId == item.Id) });
                }
            }

            LoadedProgrammes.AddRange(list);
            // Ownership
            SelectedOwnershipOption = LoadedOwnershipOptions.FirstOrDefault(x => x.Id == Registration.IsOwnedId) ?? null;
            SelectedIsBicycle = LoadedIsBicycles.FirstOrDefault(x => x.Id == Registration.IsBicycleId) ?? null;
            SelectedIsCar = LoadedIsCars.FirstOrDefault(x => x.Id == Registration.IsCarId) ?? null;
            SelectedIsMobilePhone = LoadedIsMobilePhones.FirstOrDefault(x => x.Id == Registration.IsMobilePhoneId) ?? null;
            SelectedIsMotorcycle = LoadedIsMotorcycles.FirstOrDefault(x => x.Id == Registration.IsMotorcycleId) ?? null;
            SelectedIsTukTuk = LoadedIsTukTuks.FirstOrDefault(x => x.Id == Registration.IsTukTukId) ?? null;
            SelectedIsRefrigerator = LoadedIsRefrigerators.FirstOrDefault(x => x.Id == Registration.IsRefrigeratorId) ?? null;
            SelectedIsTelevision = LoadedIsTelevisions.FirstOrDefault(x => x.Id == Registration.IsTelevisionId) ?? null;
            SelectedIsReceivingSocial = LoadedIsReceivingSocials.FirstOrDefault(x => x.Id == Registration.IsReceivingSocialId) ?? null;
            SelectedBenefitType = LoadedBenefitTypes.FirstOrDefault(x => x.Id == Registration.BenefitTypeId) ?? null;
            SelectedFamilyBenefitType = LoadedFamilyBenefitTypes.FirstOrDefault(x => x.Id == Registration.FamilyBenefitTypeId) ?? null;
            SelectedTenureStatus = LoadedTenureStatuses.FirstOrDefault(x => x.Id == Registration.TenureStatusId) ?? null;
            SelectedRoofConstructionMaterial = LoadedRoofConstructionMaterials.FirstOrDefault(x => x.Id == Registration.RoofConstructionMaterialId) ?? null;
            SelectedFloorConstructionMaterial = LoadedFloorConstructionMaterials.FirstOrDefault(x => x.Id == Registration.FloorConstructionMaterialId) ?? null;
            SelectedWallConstructionMaterial = LoadedWallConstructionMaterials.FirstOrDefault(x => x.Id == Registration.WallConstructionMaterialId) ?? null;
            SelectedDwellingRisk = LoadedDwellingRisks.FirstOrDefault(x => x.Id == Registration.DwellingUnitRiskId) ?? null;
            SelectedWaterSource = LoadedWaterSources.FirstOrDefault(x => x.Id == Registration.WaterSourceId) ?? null;
            SelectedWasteDisposal = LoadedWasteDisposals.FirstOrDefault(x => x.Id == Registration.WasteDisposalModeId) ?? null;
            SelectedCookingFuel = LoadedCookingFuels.FirstOrDefault(x => x.Id == Registration.CookingFuelTypeId) ?? null;
            SelectedLightingFuel = LoadedLightingFuels.FirstOrDefault(x => x.Id == Registration.LightingFuelTypeId) ?? null;
            SelectedHouseholdConditions = LoadedHouseholdConditions.FirstOrDefault(x => x.Id == Registration.HouseHoldConditionId) ?? null;
            SelectedBenefitType = LoadedBenefitTypes.FirstOrDefault(x => x.Id == Registration.BenefitTypeId) ?? null;
            SelectedOtherProgramme = LoadedOtherProgrammes.FirstOrDefault(x => x.Id == Registration.OtherProgrammesId) ?? null;
            SelectedNsnpBenefit = LoadedNsnpBenefits.FirstOrDefault(x => x.Id == Registration.NsnpProgrammesId) ?? null;
            SelectedSkippedMeal = LoadedSkippedMeals.FirstOrDefault(x => x.Id == Registration.IsSkippedMealId) ?? null;

            IsInKind = false;
            IsNotInKind = false;
            IsRecievingSocialItem = false;
            IsNotRecievingSocial = false;

            if (SelectedIsReceivingSocial != null)
            {
                if (SelectedIsReceivingSocial.Code == "Yes")
                {
                    IsRecievingSocialItem = true;
                    IsNotRecievingSocial = false;
                }
                else
                {
                    IsRecievingSocialItem = false;
                    IsNotRecievingSocial = false;
                }
            }
            if (SelectedIsReceivingFamily != null)
            {
                if (SelectedIsReceivingFamily.Code == "Yes")
                {
                    IsRecievingFamilyItem = true;
                    IsNotRecievingFamily = false;
                }
                else
                {
                    IsRecievingFamilyItem = false;
                    IsNotRecievingFamily = false;
                }
            }

            if (SelectedOtherProgramme != null)
            {
                IsOtherProgramme = SelectedOtherProgramme.Code == "Yes";
            }

            if (SelectedBenefitType != null)
            {
                if (SelectedBenefitType.Code == "Cash")
                {
                    IsNotInKind = true;
                    IsInKind = false;
                }
                else
                {
                    IsInKind = true;
                    IsNotInKind = false;
                }
            }

            _validator = new RegistrationValidator();
        }

        public ICommand EditHouseholdCommand => _editHouseholdCommand ?? (_editHouseholdCommand = new Command(async () => await ExecuteEditHousehold()));

        public ObservableRangeCollection<RegistrationMember> HouseholdMembers { get; } = new ObservableRangeCollection<RegistrationMember>();

        public int Id { get; set; }

        public bool IsInKind
        {
            get { return isInKind; }
            set { SetProperty(ref isInKind, value); }
        }

        public bool IsNotInKind
        {
            get => isNotInKind;
            set
            {
                if (SetProperty(ref isNotInKind, value))
                {
                    IsInKind = !isNotInKind;
                }
            }
        }

        public bool IsFamilyInKind
        {
            get { return isFamilyInKind; }
            set { SetProperty(ref isFamilyInKind, value); }
        }

        public bool IsNotFamilyInKind
        {
            get => isNotFamilyInKind;
            set
            {
                if (SetProperty(ref isNotFamilyInKind, value))
                {
                    IsInKind = !isNotFamilyInKind;
                }
            }
        }

        public bool IsNotRecievingFamily
        {
            get => isNotRecievingFamily;
            set
            {
                if (SetProperty(ref isNotRecievingFamily, value))
                {
                    IsRecievingFamilyItem = !isNotRecievingFamily;
                }
            }
        }

        public bool IsNotRecievingSocial
        {
            get => isNotRecievingSocial;
            set
            {
                if (SetProperty(ref isNotRecievingSocial, value))
                {
                    IsRecievingSocialItem = !isNotRecievingSocial;
                }
            }
        }

        public bool IsOtherProgramme
        {
            get => isOtherProgramme;
            set { SetProperty(ref isOtherProgramme, value); }
        }

        public bool IsRecievingSocialItem
        {
            get { return isRecievingSocialItem; }
            set { SetProperty(ref isRecievingSocialItem, value); }
        }

        public bool IsRecievingFamilyItem
        {
            get { return isRecievingFamilyItem; }
            set { SetProperty(ref isRecievingFamilyItem, value); }
        }

        public ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>> LoadedProgrammes
        {
            get { return _loadedProgrammes; }
            set => SetProperty(ref _loadedProgrammes, value);
        }

        //public string Message
        //{
        //    get { return message; }
        //    set { SetProperty(ref message, value); }
        //}
        public Registration Registration { get; set; }

        public ICommand SaveHouseHoldCommand => _saveHouseHoldCommand ?? (_saveHouseHoldCommand = new Command(async () => await ExecuteSaveHouseHold()));

        public SystemCodeDetail SelectedBenefitType
        {
            get => _selectedBenefitType;
            set
            {
                if (this._selectedBenefitType == value)
                {
                    return;
                }

                this._selectedBenefitType = value;
                if (_selectedBenefitType.Code == "Cash")
                {
                    IsInKind = false;
                    IsNotInKind = true;
                }
                else
                {
                    IsNotInKind = false;
                    IsInKind = true;
                }
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedFamilyBenefitType
        {
            get => _selectedFamilyBenefitType;
            set
            {
                if (this._selectedFamilyBenefitType == value)
                {
                    return;
                }

                this._selectedFamilyBenefitType = value;
                if (_selectedFamilyBenefitType.Code == "Cash")
                {
                    IsFamilyInKind = false;
                    IsNotFamilyInKind = true;
                }
                else
                {
                    IsNotFamilyInKind = false;
                    IsFamilyInKind = true;
                }
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedCookingFuel
        {
            get => _selectedCookingFuel;
            set
            {
                if (this._selectedCookingFuel == value)
                {
                    return;
                }

                this._selectedCookingFuel = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedDwellingRisk
        {
            get => _selectedDwellingRisk;
            set
            {
                if (this._selectedDwellingRisk == value)
                {
                    return;
                }

                this._selectedDwellingRisk = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedFloorConstructionMaterial
        {
            get => _selectedFloorConstructionMaterial;
            set
            {
                if (this._selectedFloorConstructionMaterial == value)
                {
                    return;
                }

                this._selectedFloorConstructionMaterial = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedHouseholdConditions
        {
            get => _selectedHouseholdConditions;
            set
            {
                if (this._selectedHouseholdConditions == value)
                {
                    return;
                }

                this._selectedHouseholdConditions = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedInterviewStatus
        {
            get => _selectedInterviewStatus;
            set
            {
                if (this._selectedInterviewStatus == value)
                {
                    return;
                }

                this._selectedInterviewStatus = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsBicycle
        {
            get => _selectedIsBicycle;
            set
            {
                if (this._selectedIsBicycle == value)
                {
                    return;
                }

                this._selectedIsBicycle = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsCar
        {
            get => _selectedIsCar;
            set
            {
                if (this._selectedIsCar == value)
                {
                    return;
                }

                this._selectedIsCar = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsMobilePhone
        {
            get => _selectedIsMobilePhone;
            set
            {
                if (this._selectedIsMobilePhone == value)
                {
                    return;
                }

                this._selectedIsMobilePhone = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsMotorcycle
        {
            get => _selectedIsMotorcycle;
            set
            {
                if (this._selectedIsMotorcycle == value)
                {
                    return;
                }

                this._selectedIsMotorcycle = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsReceivingSocial
        {
            get => _selectedIsReceivingSocial;
            set
            {
                if (this._selectedIsReceivingSocial == value)
                {
                    return;
                }

                this._selectedIsReceivingSocial = value;
                if (_selectedIsReceivingSocial.Code == "Yes")
                {
                    IsRecievingSocialItem = true;
                    IsNotRecievingSocial = false;
                }
                else
                {
                    IsNotRecievingSocial = true;
                    IsRecievingSocialItem = false;
                }
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsReceivingFamily
        {
            get => _selectedIsReceivingFamily;
            set
            {
                if (this._selectedIsReceivingFamily == value)
                {
                    return;
                }

                this._selectedIsReceivingFamily = value;
                if (_selectedIsReceivingFamily.Code == "Yes")
                {
                    IsRecievingFamilyItem = true;
                    IsNotRecievingFamily = false;
                }
                else
                {
                    IsNotRecievingSocial = true;
                    IsRecievingSocialItem = false;
                }
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsRefrigerator
        {
            get => _selectedIsRefrigerator;
            set
            {
                if (this._selectedIsRefrigerator == value)
                {
                    return;
                }

                this._selectedIsRefrigerator = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsTelevision
        {
            get => _selectedIsTelevision;
            set
            {
                if (this._selectedIsTelevision == value)
                {
                    return;
                }

                this._selectedIsTelevision = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIsTukTuk
        {
            get => _selectedIsTukTuk;
            set
            {
                if (this._selectedIsTukTuk == value)
                {
                    return;
                }

                this._selectedIsTukTuk = value;
                this.OnPropertyChanged();
            }
        }

        //public SystemCodeDetail SelectedHouseholdOptions
        //{
        //    get => _selectedHouseholdOptions;
        //    set
        //    {
        //        if (this._selectedHouseholdOptions == value) return;
        //        this._selectedHouseholdOptions = value;
        //        this.OnPropertyChanged();
        //    }
        //}
        public SystemCodeDetail SelectedLightingFuel
        {
            get => _selectedLightingFuel;
            set
            {
                if (this._selectedLightingFuel == value)
                {
                    return;
                }

                this._selectedLightingFuel = value;
                this.OnPropertyChanged();
            }
        }

        public RegistrationMember SelectedMember
        {
            get { return selectedMember; }
            set
            {
                selectedMember = value;
                OnPropertyChanged();
                if (selectedMember == null)
                {
                    return;
                }

                Navigation.PushAsync(new RegistrationMemberPage(selectedMember.RegistrationId, selectedMember.Id.ToString()));
                return;
                //selectedMember = null;
            }
        }

        public SystemCodeDetail SelectedNsnpBenefit
        {
            get => _selectedNsnpBenefit;
            set
            {
                if (this._selectedNsnpBenefit == value)
                {
                    return;
                }

                this._selectedNsnpBenefit = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<SystemCodeDetail> SelectedNsnpProgrammes
        {
            get { return _selectedNsnpProgrammes; }
            set => SetProperty(ref _selectedNsnpProgrammes, value);
        }

        public SystemCodeDetail SelectedOtherProgramme
        {
            get => _selectedOtherProgramme;
            set
            {
                if (this._selectedOtherProgramme == value)
                {
                    return;
                }

                this._selectedOtherProgramme = value;
                IsOtherProgramme = _selectedOtherProgramme.Code == "Yes";

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedOwnershipOption
        {
            get => _selectedOwnershipOption;
            set
            {
                if (this._selectedOwnershipOption == value)
                {
                    return;
                }

                LoadedTenureStatuses.Clear();
                this._selectedOwnershipOption = value;
                LoadedTenureStatuses.AddRange(this._selectedOwnershipOption.Code == "Yes"
                    ? App.Database.SystemCodeDetailsGetByCode("Tenure Status").OrderBy(x => x.Id).Take(3)
                    : App.Database.SystemCodeDetailsGetByCode("Tenure Status").OrderBy(x => x.Id).Skip(3).Take(7));
                this.OnPropertyChanged();
            }
        }

        //    return list;
        //}
        public SystemCodeDetail SelectedRoofConstructionMaterial
        {
            get => _selectedRoofConstructionMaterial;
            set
            {
                if (this._selectedRoofConstructionMaterial == value)
                {
                    return;
                }

                this._selectedRoofConstructionMaterial = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedSkippedMeal
        {
            get => _selectedSkippedMeal;
            set
            {
                if (this._selectedSkippedMeal == value)
                {
                    return;
                }

                this._selectedSkippedMeal = value;

                this.OnPropertyChanged();
            }
        }

        //    foreach (var data in NsnpProgrammesList)
        //        list.Add(new SelectableData<SystemCodeDetail>() { Data = data.Data, Selected = data.Selected });
        public SystemCodeDetail SelectedTenureStatus
        {
            get => _selectedTenureStatus;
            set
            {
                if (this._selectedTenureStatus == value)
                {
                    return;
                }

                this._selectedTenureStatus = value;
                this.OnPropertyChanged();
            }
        }

        //public List<SelectableData<SystemCodeDetail>> GetNewNsnpProgrammesList()
        //{
        //    var list = new List<SelectableData<SystemCodeDetail>>();
        public SystemCodeDetail SelectedWallConstructionMaterial
        {
            get => _selectedWallConstructionMaterial;
            set
            {
                if (this._selectedWallConstructionMaterial == value)
                {
                    return;
                }

                this._selectedWallConstructionMaterial = value;
                this.OnPropertyChanged();
            }
        }

        //public List<SelectableData<SystemCodeDetail>> NsnpProgrammesList { get; set; }
        public SystemCodeDetail SelectedWasteDisposal
        {
            get => _selectedWasteDisposal;
            set
            {
                if (this._selectedWasteDisposal == value)
                {
                    return;
                }

                this._selectedWasteDisposal = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedWaterSource
        {
            get => _selectedWaterSource;
            set
            {
                if (this._selectedWaterSource == value)
                {
                    return;
                }

                this._selectedWaterSource = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<RegistrationMember> GetHouseholdMembers(int id)
        {
            var items = App.Database.GetTableRows("RegistrationMember", "RegistrationId", id.ToString());
            var hh = new ObservableCollection<RegistrationMember>();
            foreach (var item in items)
            {
                hh.Add((RegistrationMember)item);
            }
            return hh;
        }

        private async Task ExecuteEditHousehold()
        {
            try
            {
                await Navigation.PushAsync(new RegistrationEditPage(Registration.Id));
            }
            catch (Exception e)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(
                    MessageKeys.Error,
                    new MessagingServiceAlert
                    {
                        Title = "Please  Try Again!!",
                        Message = e.Message,
                        Cancel = "OK"
                    });
                return;
            }
        }

        private async Task ExecuteSaveHouseHold()
        {
            try
            {
                IsBusy = true;
                Message = "Validating .. ";
                var registration = this.Registration;
                registration.EndTime = DateFormatter.ToSQLiteDateTimeString(DateTime.Now); ;

                var errorMessage = "";
                if (SelectedOwnershipOption == null)
                {
                    errorMessage += "Check (2.01)\n";
                }
                else
                {
                    registration.IsOwnedId = SelectedOwnershipOption.Id;
                }

                if (SelectedTenureStatus == null)
                {
                    errorMessage += "Check (2.02) \n";
                }
                else
                {
                    registration.TenureStatusId = SelectedTenureStatus.Id;
                }

                if (SelectedRoofConstructionMaterial == null)
                {
                    errorMessage += "Check (2.03) \n";
                }
                else
                {
                    registration.RoofConstructionMaterialId = SelectedRoofConstructionMaterial.Id;
                }

                if (SelectedWallConstructionMaterial == null)
                {
                    errorMessage += "Check (2.04)\n";
                }
                else
                {
                    registration.WallConstructionMaterialId = SelectedWallConstructionMaterial.Id;
                }

                if (SelectedFloorConstructionMaterial == null)
                {
                    errorMessage += "Check (2.05) \n";
                }
                else
                {
                    registration.FloorConstructionMaterialId = SelectedFloorConstructionMaterial.Id;
                }

                if (SelectedDwellingRisk == null)
                {
                    errorMessage += "Check (2.06) \n";
                }
                else
                {
                    registration.DwellingUnitRiskId = SelectedDwellingRisk.Id;
                }

                if (SelectedWaterSource == null)
                {
                    errorMessage += "Check (2.07)\n";
                }
                else
                {
                    registration.WaterSourceId = SelectedWaterSource.Id;
                }

                if (SelectedWasteDisposal == null)
                {
                    errorMessage += "Check (2.08) \n";
                }
                else
                {
                    registration.WasteDisposalModeId = SelectedWasteDisposal.Id;
                }

                if (SelectedCookingFuel == null)
                {
                    errorMessage += "Check (2.09) \n";
                }
                else
                {
                    registration.CookingFuelTypeId = SelectedCookingFuel.Id;
                }

                if (SelectedLightingFuel == null)
                {
                    errorMessage += "Check (2.10) \n";
                }
                else
                {
                    registration.LightingFuelTypeId = SelectedLightingFuel.Id;
                }

                if (SelectedIsTelevision == null)
                {
                    errorMessage += "Check (2.11) \n";
                }
                else
                {
                    registration.IsTelevisionId = SelectedIsTelevision.Id;
                }

                if (SelectedIsMotorcycle == null)
                {
                    errorMessage += "Check (2.12) \n";
                }
                else
                {
                    registration.IsMotorcycleId = SelectedIsMotorcycle.Id;
                }

                if (SelectedIsTukTuk == null)
                {
                    errorMessage += "Check (2.13) \n";
                }
                else
                {
                    registration.IsTukTukId = SelectedIsTukTuk.Id;
                }

                if (SelectedIsRefrigerator == null)
                {
                    errorMessage += "Check (2.14) \n";
                }
                else
                {
                    registration.IsRefrigeratorId = SelectedIsRefrigerator.Id;
                }

                if (SelectedIsCar == null)
                {
                    errorMessage += "Check (2.15) \n";
                }
                else
                {
                    registration.IsCarId = SelectedIsCar.Id;
                }

                if (SelectedIsMobilePhone == null)
                {
                    errorMessage += "Check (2.16) \n";
                }
                else
                {
                    registration.IsMobilePhoneId = SelectedIsMobilePhone.Id;
                }

                if (SelectedIsBicycle == null)
                {
                    errorMessage += "Check (2.17) \n";
                }
                else
                {
                    registration.IsBicycleId = SelectedIsBicycle.Id;
                }

                if (SelectedHouseholdConditions == null)
                {
                    errorMessage += "Check (2.28) \n";
                }
                else
                {
                    registration.HouseHoldConditionId = SelectedHouseholdConditions.Id;
                }

                if (SelectedSkippedMeal == null)
                {
                    errorMessage += "Check (2.29) \n";
                }
                else
                {
                    registration.IsSkippedMealId = SelectedSkippedMeal.Id;
                }

                if (SelectedOtherProgramme == null)
                {
                    errorMessage += "Check (2.30) \n";
                }
                else
                {
                    registration.OtherProgrammesId = SelectedOtherProgramme.Id;
                }

                if (SelectedIsReceivingSocial == null)
                {
                    errorMessage += "Check (2.31) \n";
                }
                else
                {
                    registration.IsReceivingSocialId = SelectedIsReceivingSocial.Id;
                }

                if (SelectedBenefitType != null)
                {
                    registration.BenefitTypeId = SelectedBenefitType.Id;
                }

                //if (string.IsNullOrEmpty(registration.Village))
                //{
                //    errorMessage += "Village is required \n";
                //}

                if (SelectedIsReceivingFamily != null)
                {
                    registration.IsReceivingFamilyId = SelectedIsReceivingFamily.Id;
                }

                if (SelectedFamilyBenefitType != null)
                {
                    registration.FamilyBenefitTypeId = SelectedFamilyBenefitType.Id;
                }

                //if (string.IsNullOrEmpty(registration.Village))
                //{
                //    errorMessage += "Village is required \n";
                //}

                //if (string.IsNullOrEmpty(registration.NearestReligiousBuilding))
                //{
                //    errorMessage += "Nearest Church or Mosque is required \n";
                //}

                //if (string.IsNullOrEmpty(registration.NearestSchool))
                //{
                //    errorMessage += "Nearest School is required \n";
                //}

                if (registration.HabitableRooms < 1)
                {
                    errorMessage += "The Habitable Rooms must be greater than 0 \n";
                }

                if (registration.HouseholdMembers < 1)
                {
                    errorMessage += "The Household Members must be greater than 0 \n";
                }

                var validationResult = _validator.Validate(registration);
                if (validationResult.IsValid && errorMessage == "")
                {
                    registration.EndTime = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                    registration.RegistrationDate = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);

                    registration.EnumeratorId = Settings.Current.EnumeratorId;
                    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Ongoing").Id;
                    registration.InterviewStatusId = InterviewStatusId;

                    Message = "Updating Database .. ";
                    App.Database.AddOrUpdate(registration);

                    var existingProgrammes = App.Database.GetTableRows<RegistrationProgramme>("RegistrationProgramme", "RegistrationId", Registration.Id.ToString());

                    if (existingProgrammes.Any())
                    {
                        foreach (var item in existingProgrammes)
                        {
                            App.Database.Delete(item);
                        }
                    }

                    var items = GetSelectedNsnpProgrammes();

                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            var rp = new RegistrationProgramme
                            {
                                ProgrammeId = item.Id,
                                RegistrationId = registration.Id
                            };
                            App.Database.Create(rp);
                        }
                    }
                    Message = "Detecting GPS Coordinates... ";
                    var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
                    registration.Latitude = position.Latitude;
                    registration.Longitude = position.Longitude;
                    Message = "Updating Database .. ";
                    App.Database.AddOrUpdate(registration);
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                    {
                        Title = "Success",
                        Message = "Household information saved successfully.",
                        Cancel = "OK"
                    });
                    await Navigation.PopAsync(true);
                    //await Navigation.PushAsync(new RegistrationDetailPage(registration.Id));
                    await ((MainPage)App.Current.MainPage).Detail.Navigation.PushAsync(new RegistrationDetailPage(registration.Id));
                }
                else
                {
                    ValidateMessage = GetErrorListFromValidationResult(validationResult).Replace(" id'", "");

                    if (errorMessage.Length > 0 || ValidateMessage.Length > 0)
                    {
                        ValidateMessage = $"{ValidateMessage}\n{errorMessage}";
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(
                            MessageKeys.Error,
                            new MessagingServiceAlert
                            {
                                Title = "Please Check the Data and Try Again!!",
                                Message = ValidateMessage,
                                Cancel = "OK"
                            });
                        IsBusy = false;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("A task"))
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Error!!",
                            Message = "Unable to detect GPS coordinates. Make sure location service is turned on, or move outside or near a window.",
                            Cancel = "OK"
                        });
                }
                else
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Please Correct the Data and Try Again!!",
                            Message = e.Message,
                            Cancel = "OK"
                        });
                }
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;
            }
            return;
        }

        private ObservableCollection<SystemCodeDetail> GetSelectedNsnpProgrammes()
        {
            var selected = LoadedProgrammes
                .Where(p => p.IsSelected)
                .Select(p => p.Item)
                .ToList();
            return new ObservableCollection<SystemCodeDetail>(selected);
        }

        #region Options

        public ObservableRangeCollection<SystemCodeDetail> IsBicycles = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsCars = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsMobilePhones = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsMotorcycles = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsRefrigerators = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsTelevisions = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> IsTukTuks = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> OwnerOccupieds = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> OwnershipOptions = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> LoadedBenefitTypes
        {
            get => BenefitTypes;
            set => SetProperty(ref BenefitTypes, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedFamilyBenefitTypes
        {
            get => FamilyBenefitTypes;
            set => SetProperty(ref FamilyBenefitTypes, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedCookingFuels
        {
            get => CookingFuels;
            set => SetProperty(ref CookingFuels, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedDwellingRisks
        {
            get => DwellingRisks;
            set => SetProperty(ref DwellingRisks, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedFloorConstructionMaterials
        {
            get => FloorConstructionMaterials;
            set => SetProperty(ref FloorConstructionMaterials, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedHouseholdConditions
        {
            get => HouseholdConditions;
            set => SetProperty(ref HouseholdConditions, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedInterviewStatus
        {
            get => InterviewStatus;
            set => SetProperty(ref InterviewStatus, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsBicycles
        {
            get => IsBicycles;
            set => SetProperty(ref IsBicycles, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsCars
        {
            get => IsCars;
            set => SetProperty(ref IsCars, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsMobilePhones
        {
            get => IsMobilePhones;
            set => SetProperty(ref IsMobilePhones, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsMotorcycles
        {
            get => IsMotorcycles;
            set => SetProperty(ref IsMotorcycles, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsReceivingSocials
        {
            get => IsReceivingSocials;
            set => SetProperty(ref IsReceivingSocials, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsReceivingFamilys
        {
            get => IsReceivingFamilys;
            set => SetProperty(ref IsReceivingFamilys, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsRefrigerators
        {
            get => IsRefrigerators;
            set => SetProperty(ref IsRefrigerators, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsTelevisions
        {
            get => IsTelevisions;
            set => SetProperty(ref IsTelevisions, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIsTukTuks
        {
            get => IsTukTuks;
            set => SetProperty(ref IsTukTuks, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedLightingFuels
        {
            get => LightingFuels;
            set => SetProperty(ref LightingFuels, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedNsnpBenefits
        {
            get => NsnpBenefits;
            set => SetProperty(ref NsnpBenefits, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedOtherProgrammes
        {
            get => OtherProgrammes;
            set => SetProperty(ref OtherProgrammes, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedOwnerOccupieds
        {
            get => OwnerOccupieds;
            set => SetProperty(ref OwnerOccupieds, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedOwnershipOptions
        {
            get => OwnershipOptions;
            set => SetProperty(ref OwnershipOptions, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedRoofConstructionMaterials
        {
            get => RoofConstructionMaterials;
            set => SetProperty(ref RoofConstructionMaterials, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedSkippedMeals
        {
            get => SkippedMeals;
            set => SetProperty(ref SkippedMeals, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedTenureStatuses
        {
            get => TenureStatuses;
            set => SetProperty(ref TenureStatuses, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedWallConstructionMaterials
        {
            get => WallConstructionMaterials;
            set => SetProperty(ref WallConstructionMaterials, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedWasteDisposals
        {
            get => WasteDisposals;
            set => SetProperty(ref WasteDisposals, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedWaterSources
        {
            get => WaterSources;
            set => SetProperty(ref WaterSources, value);
        }

        //        //LoadedSkippedMeal

        #endregion Options
    }
}