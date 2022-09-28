using MCHMIS.Mobile.Converters;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Models;
using MCHMIS.Mobile.Pages;
using FormsToolkit;
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
    public class RegistrationDetailViewModel : LocalBaseViewModel
    {
        public RegistrationDetailViewModel(INavigation navigation, int id) : base(navigation)
        {
            var data = App.Database.GetTableRow("Registration", "Id", id.ToString());
            Registration = (Registration)data;

            // Registration.SubLocation = (SubLocation)App.Database.GetTableRow("SubLocation", "id", Registration.SubLocationId.ToString());
            // Registration.CommunityArea = (CommunityArea)App.Database.GetTableRow("CommunityArea", "Id", Registration.CommunityAreaId.ToString());
            Registration.Village = (Village)App.Database.GetTableRow("Village", "Id", Registration.VillageId.ToString());
            Registration.Ward = (Ward)App.Database.GetTableRow("Ward", "Id", Registration.WardId.ToString());
            Navigation = navigation;

            var systemCodeDetails = App.Database.GetTableRows<SystemCodeDetail>("SystemCodeDetail");

            var otherP = App.Database.SystemCodeDetailsGetByCode("Other SP Programme").ToList();
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

            //   LoadedProgrammes.AddRange(list.Where(x => x.IsSelected));

            if (Registration.IsOwnedId > 0)
            {
                Registration.IsOwned = systemCodeDetails.Single(x => x.Id == Registration.IsOwnedId);
            }

            if (Registration.TenureStatusId > 0)
            {
                Registration.TenureStatus = systemCodeDetails.Single(x => x.Id == Registration.TenureStatusId);
            }

            if (Registration.RoofConstructionMaterialId > 0)
            {
                Registration.RoofConstructionMaterial = systemCodeDetails.Single(x => x.Id == Registration.RoofConstructionMaterialId);
            }

            if (Registration.WallConstructionMaterialId > 0)
            {
                Registration.WallConstructionMaterial = systemCodeDetails.Single(x => x.Id == Registration.WallConstructionMaterialId);
            }

            if (Registration.FloorConstructionMaterialId > 0)
            {
                Registration.FloorConstructionMaterial = systemCodeDetails.Single(x => x.Id == Registration.FloorConstructionMaterialId);
            }

            if (Registration.DwellingUnitRiskId > 0)
            {
                Registration.DwellingUnitRisk = systemCodeDetails.Single(x => x.Id == Registration.DwellingUnitRiskId);
            }

            if (Registration.WaterSourceId > 0)
            {
                Registration.WaterSource = systemCodeDetails.Single(x => x.Id == Registration.WaterSourceId);
            }

            if (Registration.WasteDisposalModeId > 0)
            {
                Registration.WasteDisposalMode = systemCodeDetails.Single(x => x.Id == Registration.WasteDisposalModeId);
            }

            if (Registration.CookingFuelTypeId > 0)
            {
                Registration.CookingFuelType = systemCodeDetails.Single(x => x.Id == Registration.CookingFuelTypeId);
            }

            if (Registration.LightingFuelTypeId > 0)
            {
                Registration.LightingFuelType = systemCodeDetails.Single(x => x.Id == Registration.LightingFuelTypeId);
            }

            if (Registration.IsTelevisionId > 0)
            {
                Registration.IsTelevision = systemCodeDetails.Single(x => x.Id == Registration.IsTelevisionId);
            }

            if (Registration.IsTelevisionId > 0)
            {
                Registration.IsTelevision = systemCodeDetails.Single(x => x.Id == Registration.IsTelevisionId);
            }

            if (Registration.IsMotorcycleId > 0)
            {
                Registration.IsMotorcycle = systemCodeDetails.Single(x => x.Id == Registration.IsMotorcycleId);
            }

            if (Registration.IsTukTukId > 0)
            {
                Registration.IsTukTuk = systemCodeDetails.Single(x => x.Id == Registration.IsTukTukId);
            }

            if (Registration.IsRefrigeratorId > 0)
            {
                Registration.IsRefrigerator = systemCodeDetails.Single(x => x.Id == Registration.IsRefrigeratorId);
            }

            if (Registration.IsCarId > 0)
            {
                Registration.IsCar = systemCodeDetails.Single(x => x.Id == Registration.IsCarId);
            }

            if (Registration.IsMobilePhoneId > 0)
            {
                Registration.IsMobilePhone = systemCodeDetails.Single(x => x.Id == Registration.IsMobilePhoneId);
            }

            if (Registration.IsBicycleId > 0)
            {
                Registration.IsBicycle = systemCodeDetails.Single(x => x.Id == Registration.IsBicycleId);
            }

            if (Registration.HouseHoldConditionId > 0)
            {
                Registration.HouseHoldCondition = systemCodeDetails.Single(x => x.Id == Registration.HouseHoldConditionId);
            }

            if (Registration.IsSkippedMealId > 0)
            {
                Registration.IsSkippedMeal = systemCodeDetails.Single(x => x.Id == Registration.IsSkippedMealId);
            }

            if (Registration.NsnpProgrammesId > 0)
            {
                Registration.NsnpProgrammes = systemCodeDetails.Single(x => x.Id == Registration.NsnpProgrammesId);
            }

            if (Registration.IsReceivingSocialId > 0)
            {
                Registration.IsReceivingSocial = systemCodeDetails.Single(x => x.Id == Registration.IsReceivingSocialId);
            }
            if (Registration.IsReceivingFamilyId > 0)
            {
                Registration.IsReceivingFamily = systemCodeDetails.Single(x => x.Id == Registration.IsReceivingFamilyId);
            }

            if (Registration.OtherProgrammesId > 0)
            {
                Registration.OtherProgrammes = systemCodeDetails.Single(x => x.Id == Registration.OtherProgrammesId);
            }

            if (Registration.BenefitTypeId > 0)
            {
                Registration.BenefitType = systemCodeDetails.Single(x => x.Id == Registration.BenefitTypeId);
            }
            if (Registration.FamilyBenefitTypeId > 0)
            {
                Registration.FamilyBenefitType = systemCodeDetails.Single(x => x.Id == Registration.FamilyBenefitTypeId);
            }

            if (Registration.InterviewStatusId != null && Registration.InterviewStatusId > 0)
            {
                Registration.InterviewStatus = systemCodeDetails.Single(x => x.Id == Registration.InterviewStatusId);
            }

            if (Registration.InterviewResultId != null && Registration.InterviewResultId > 0)
            {
                Registration.InterviewResult = systemCodeDetails.Single(x => x.Id == Registration.InterviewResultId);
            }

            IsComplete = Registration.InterviewStatus.Code == "Completed";
            IsPending = Registration.InterviewStatus.Code == "Pending";
            IsOngoing = Registration.InterviewStatus.Code == "Ongoing";
            IsPendingOrOngoing = Registration.InterviewStatus.Code == "Ongoing" || Registration.InterviewStatus.Code == "Pending";
            IsNotComplete = Registration.InterviewStatus.Code == "Ongoing" || Registration.InterviewStatus.Code == "Pending";

            HouseholdMembers.AddRange(GetHouseholdMembers(Registration.Id));

            Id = id;
        }

        public bool IsPendingOrOngoing
        {
            get { return isPendingOrOngoing; }
            set { SetProperty(ref isPendingOrOngoing, value); }
        }

        public bool IsNotPendingOrOngoing
        {
            get => isNotPendingOrOngoing;
            set
            {
                if (SetProperty(ref isNotPendingOrOngoing, value))
                {
                    IsPendingOrOngoing = !isNotPendingOrOngoing;
                }
            }
        }

        private bool isPendingOrOngoing;
        private bool isNotPendingOrOngoing;

        public bool IsShowNoOneAtHome
        {
            get { return isShowNoOneAtHome; }
            set { SetProperty(ref isShowNoOneAtHome, value); }
        }

        public bool IsNotShowNoOneAtHome
        {
            get => isNotShowNoOneAtHome;
            set
            {
                if (SetProperty(ref isNotShowNoOneAtHome, value))
                {
                    IsShowNoOneAtHome = !isNotShowNoOneAtHome;
                }
            }
        }

        private bool isShowNoOneAtHome;

        private bool isNotShowNoOneAtHome;

        public bool IsComplete
        {
            get { return isComplete; }
            set { SetProperty(ref isComplete, value); }
        }

        public bool IsNotComplete
        {
            get => isNotComplete;
            set
            {
                if (SetProperty(ref isNotComplete, value))
                {
                    IsComplete = !isNotComplete;
                }
            }
        }

        private bool isComplete;

        private bool isNotComplete;

        public bool IsOngoing
        {
            get { return isOngoing; }
            set { SetProperty(ref isOngoing, value); }
        }

        public bool IsNotOngoing
        {
            get => isNotOngoing;
            set
            {
                if (SetProperty(ref isNotOngoing, value))
                {
                    IsOngoing = !isNotOngoing;
                }
            }
        }

        private bool isOngoing;

        private bool isNotOngoing;

        public bool IsPending
        {
            get { return isPending; }
            set { SetProperty(ref isPending, value); }
        }

        public bool IsNotPending
        {
            get => isNotPending;
            set
            {
                if (SetProperty(ref isNotPending, value))
                {
                    IsPending = !isNotPending;
                }
            }
        }

        private bool isPending;
        private bool isNotPending;

        public INavigation Navigation;
        public Registration Registration { get; set; }
        public int Id { get; set; }

        public ObservableRangeCollection<RegistrationMember> HouseholdMembers { get; } = new ObservableRangeCollection<RegistrationMember>();

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

        public ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>> _loadedProgrammes =
            new ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>>();

        public ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>> LoadedProgrammes
        {
            get { return _loadedProgrammes; }
            set => SetProperty(ref _loadedProgrammes, value);
        }

        private ObservableCollection<SystemCodeDetail> _selectedNsnpProgrammes;

        public ObservableCollection<SystemCodeDetail> SelectedNsnpProgrammes
        {
            get { return _selectedNsnpProgrammes; }
            set => SetProperty(ref _selectedNsnpProgrammes, value);
        }

        private ObservableCollection<SystemCodeDetail> GetSelectedNsnpProgrammes()
        {
            var selected = LoadedProgrammes
                .Where(p => p.IsSelected)
                .Select(p => p.Item)
                .ToList();
            return new ObservableCollection<SystemCodeDetail>(selected);
        }

        private RegistrationMember selectedMember;

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
                selectedMember = null;
            }
        }

        private ICommand _editHouseholdCommand;

        public ICommand EditHouseholdCommand => _editHouseholdCommand ?? (_editHouseholdCommand = new Command(async () => await ExecuteEditHousehold()));

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

        private ICommand _completeHouseholdCommand;

        public ICommand CompleteHouseholdCommand => _completeHouseholdCommand ?? (_completeHouseholdCommand = new Command(async () => await ExecuteCompleteHousehold()));

        private async Task ExecuteCompleteHousehold()
        {
            try
            {
                var persons = App.Database
                    .GetTableRows<RegistrationMember>("RegistrationMember", "RegistrationId",
                        Registration.Id.ToString()).Count(x => x.RelationshipId == null);

                if (persons > 0)
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Please  Try Again!!",
                            Message = " There are Household members that have not been filled. Check all the Members Data and try again",
                            Cancel = "OK"
                        });
                    return;
                }
                var status = App.Database.SystemCodeDetailGetById(Registration.InterviewStatusId.Value);

                if (status.Code == "Pending")
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Please  Try Again!!",
                            Message = " The House Interview is Pending. Kindly Fill the Form and or update household details accordingly.",
                            Cancel = "OK"
                        });
                    return;
                }

                var date = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Completed").Id;

                Registration.InterviewStatusId = InterviewStatusId;
                var members = App.Database.GetTableRows<RegistrationMember>("RegistrationMember", "RegistrationId", Registration.Id.ToString()).Count();

                if (Registration.IsTelevisionId > 0 || Registration.TenureStatusId > 0)
                {
                    if (members < Registration.HouseholdMembers)
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(
                            MessageKeys.Error,
                            new MessagingServiceAlert
                            {
                                Title = "Please  Try Again!!",
                                Message = $" {members} household members added instead of {Registration.HouseholdMembers}. Add all the members indicated on the Household including the mother's details.",
                                Cancel = "OK"
                            });
                        return;
                    }

                    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "Completed").Id;

                    Registration.InterviewResultId = InterviewResultId;
                }

                Registration.RegistrationDate = date;
                App.Database.AddOrUpdate(Registration);

                MessagingService.Current.SendMessage<MessagingServiceAlert>(
                    MessageKeys.Error,
                    new MessagingServiceAlert
                    {
                        Title = "Success",
                        Message = " The House has been Marked as Complete. This is now ready for upload to the server.",
                        Cancel = "OK"
                    });
                await ((MainPage)App.Current.MainPage).Detail.Navigation.PushAsync(new RegistrationListPage());
                return;
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

        private ICommand _emptyHouseholdCommand;

        public ICommand EmptyHouseholdCommand => _emptyHouseholdCommand ?? (_emptyHouseholdCommand = new Command(async () => await ExecuteEmptyHousehold()));

        private async Task ExecuteEmptyHousehold()
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.PushAsync(new ConfirmNoOneAtHome(Registration));
        }

        private ICommand _locateHouseholdCommand;

        public ICommand LocateHouseholdCommand => _locateHouseholdCommand ?? (_locateHouseholdCommand = new Command(async () => await ExecuteLocateHousehold()));

        private async Task ExecuteLocateHousehold()
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.PushAsync(new Reason(Registration));
            //try
            //{
            //    var date = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);

            //    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Completed").Id;
            //    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "Cannot find household").Id;

            //    Registration.InterviewStatusId = InterviewStatusId;
            //    Registration.InterviewResultId = InterviewResultId;
            //    Registration.RegistrationDate = date;

            //    App.Database.AddOrUpdate(Registration);

            //    MessagingService.Current.SendMessage<MessagingServiceAlert>(
            //        MessageKeys.Error,
            //        new MessagingServiceAlert
            //        {
            //            Title = "Please  Try Again!!",
            //            Message = "The House has been Marked as one that could not be Located. This is Now Ready to Sync",
            //            Cancel = "OK"
            //        });
            //}
            //catch (Exception e)
            //{
            //    MessagingService.Current.SendMessage<MessagingServiceAlert>(
            //        MessageKeys.Error,
            //        new MessagingServiceAlert
            //        {
            //            Title = "Please  Try Again!!",
            //            Message = e.Message,
            //            Cancel = "OK"
            //        });
            //    return;
            //}
        }

        private ICommand _newHouseholdMemberCommand;

        public ICommand NewHouseholdMemberCommand => _newHouseholdMemberCommand ??
                                                     (_newHouseholdMemberCommand = new Command(async () => await ExecuteNewHouseholdMember()));

        private async Task ExecuteNewHouseholdMember()
        {
            try
            {
                await Navigation.PushAsync(new RegistrationMemberPage(Registration.Id, null));
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

        //private ICommand _editRegistrationCommand;
        //public ICommand EditRegistrationCommand => _editRegistrationCommand ??
        //                                           (_editRegistrationCommand = new Command(async () => await ExecuteEditRegistration()));

        //private async Task ExecuteEditRegistration()
        //{
        //    try
        //    {
        //        var reg = this.Registration;
        //        await Navigation.PushAsync(new RegistrationEditPage(reg.Id));
        //    }
        //    catch (Exception e)
        //    {
        //        MessagingService.Current.SendMessage<MessagingServiceAlert>(
        //            MessageKeys.Error,
        //            new MessagingServiceAlert
        //            {
        //                Title = "Please  Try Again!!",
        //                Message = e.Message,
        //                Cancel = "OK"
        //            });
        //        return;
        //    }
        //}
    }
}