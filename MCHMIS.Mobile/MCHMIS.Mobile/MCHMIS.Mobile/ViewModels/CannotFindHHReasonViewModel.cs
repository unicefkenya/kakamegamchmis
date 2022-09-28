using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FormsToolkit;
using MCHMIS.Mobile.Converters;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Pages;
using MvvmHelpers;
using Plugin.Geolocator;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class CannotFindHHReasonViewModel : LocalBaseViewModel
    {
        private ICommand _locateHouseholdCommand;
        private Registration Registration;
        private INavigation navigation;
        private ObservableRangeCollection<SystemCodeDetail> _reasons = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> Reasons
        {
            get => _reasons;
            set => SetProperty(ref _reasons, value);
        }

        private SystemCodeDetail _selectedReason;

        public SystemCodeDetail SelectedReason
        {
            get => _selectedReason;
            set => SetProperty(ref _selectedReason, value);
        }

        public string message = string.Empty;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public CannotFindHHReasonViewModel(INavigation nav, Registration registration) : base(nav)
        {
            navigation = nav;
            var reasons = App.Database.SystemCodeDetailsGetByCode("Cannot Find Households Reasons").ToList();
            Reasons = new ObservableRangeCollection<SystemCodeDetail>();
            Reasons.AddRange(reasons);
            Registration = registration;
        }

        public ICommand LocateHouseholdCommand => _locateHouseholdCommand ??
                                                  (_locateHouseholdCommand = new Command(async () => await ExecuteLocateHousehold()));

        private async Task ExecuteLocateHousehold()
        {
            if (SelectedReason == null)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(
                    MessageKeys.Error,
                    new MessagingServiceAlert
                    {
                        Title = "Please  Try Again!!",
                        Message = "Reason is required!",
                        Cancel = "OK"
                    });
            }
            else
            {
                try
                {
                    IsBusy = true;
                    Message = "Detecting Geolocation Coordinates... ";
                    var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
                    Message = "Saving record.. ";

                    Registration.Latitude = position.Latitude;
                    Registration.Longitude = position.Longitude;

                    var date = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);

                    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Completed").Id;
                    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "Cannot find household").Id;

                    Registration.InterviewStatusId = InterviewStatusId;
                    Registration.InterviewResultId = InterviewResultId;
                    Registration.RegistrationDate = date;
                    Registration.CannotFindHouseholdReasonId = SelectedReason.Id;

                    App.Database.AddOrUpdate(Registration);

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Success",
                            Message = "The House has been Marked as one that could not be Located. This is Now Ready to Sync",
                            Cancel = "OK"
                        });
                    await navigation.PopPopupAsync();
                    await ((MainPage)App.Current.MainPage).Detail.Navigation.PushAsync(new RegistrationListPage());
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
                                Title = "Please  Try Again!!",
                                Message = e.Message,
                                Cancel = "OK"
                            });
                    }
                }
            }
        }

        private ICommand _emptyHouseholdCommand;
        public ICommand EmptyHouseholdCommand => _emptyHouseholdCommand ?? (_emptyHouseholdCommand = new Command(async () => await ExecuteEmptyHousehold()));

        private async Task ExecuteEmptyHousehold()
        {
            try
            {
                var date = DateFormatter.ToSQLiteDateTimeString(DateTime.Now.Date);
                IsBusy = true;
                Message = "Detecting Geolocation Coordinates... ";
                var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
                Message = "Saving record.. ";
                Registration.EndTime = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                Registration.RegistrationDate = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                Registration.Latitude = position.Latitude;
                Registration.Longitude = position.Longitude;
                Registration.EnumeratorId = Settings.Current.EnumeratorId;

                if (string.IsNullOrEmpty(Registration.RegDate1))
                {
                    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Pending").Id;
                    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "No one at home").Id;
                    ;
                    Registration.InterviewStatusId = InterviewStatusId;
                    Registration.InterviewResultId = InterviewResultId;
                    Registration.RegDate1 = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);

                    App.Database.AddOrUpdate(Registration);

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Updated Successfully",
                            Message = " The House has been Marked as No One at Home. Please Ensure you visit the Household again.",
                            Cancel = "OK"
                        });
                }
                else if (!string.IsNullOrEmpty(Registration.RegDate1) && string.IsNullOrEmpty(Registration.RegDate2) && string.IsNullOrEmpty(Registration.RegDate3))
                {
                    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Pending").Id;
                    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "No one at home").Id;
                    ;
                    Registration.InterviewStatusId = InterviewStatusId;
                    Registration.InterviewResultId = InterviewResultId;
                    Registration.RegDate2 = date;
                    App.Database.AddOrUpdate(Registration);

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Updated Successfully",
                            Message = " The House has been Marked as No One at Home for the Second Time. Please Ensure you visit the Household another day.",
                            Cancel = "OK"
                        });
                }
                else if (string.IsNullOrEmpty(Registration.RegDate3) && !string.IsNullOrEmpty(Registration.RegDate2) && !string.IsNullOrEmpty(Registration.RegDate1))
                {
                    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Completed").Id;
                    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "No one at home").Id;
                    ;
                    Registration.InterviewStatusId = InterviewStatusId;
                    Registration.InterviewResultId = InterviewResultId;
                    Registration.RegDate3 = date;
                    Registration.RegistrationDate = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                    App.Database.AddOrUpdate(Registration);

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                        MessageKeys.Error,
                        new MessagingServiceAlert
                        {
                            Title = "Updated Successfully",
                            Message = " The House has been Marked as No One at Home.  This House is ready to Upload.",
                            Cancel = "OK"
                        });
                }
                await navigation.PopPopupAsync();
                await ((MainPage)App.Current.MainPage).Detail.Navigation.PushAsync(new RegistrationListPage());

                //if (string.IsNullOrEmpty(Registration.RegDate3) && Registration.RegDate2 != date)
                //{
                //    int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "01").Id;
                //    int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "04").Id;

                //    Registration.InterviewStatusId = InterviewStatusId;
                //    Registration.InterviewResultId = InterviewResultId;
                //    Registration.RegDate2 = date;
                //    App.Database.AddOrUpdate(Registration);

                //    MessagingService.Current.SendMessage<MessagingServiceAlert>(
                //        MessageKeys.Error,
                //        new MessagingServiceAlert
                //        {
                //            Title = "Updated Successfully",
                //            Message = " The House has been Marked as No One at Home. Please Ensure you visit the Household another day.",
                //            Cancel = "OK"
                //        });
                //}
            }
            catch (Exception e)
            {
                IsBusy = false;
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
                            Title = "Please  Try Again!!",
                            Message = e.Message,
                            Cancel = "OK"
                        });
                }
            }
        }
    }
}