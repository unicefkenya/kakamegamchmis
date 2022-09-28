using FormsToolkit;
using MCHMIS.Mobile.Converters;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Interface;
using MCHMIS.Mobile.Models;
using Plugin.Connectivity;
using Plugin.DeviceInfo;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class SyncViewModel : LocalBaseViewModel
    {
        #region Properties

        private IAppClient client;
        public INavigation Navigation;

        private string message;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        public SyncViewModel(INavigation navigation) : base(navigation)
        {
            client = DependencyService.Get<IAppClient>();
            email = Settings.Current.Email;
            password = Settings.Current.Password;

            SyncedDownload = Settings.Current.LastSyncDown;
            SyncedUpload = Settings.Current.LastSync;

            SyncDownRegistrationText = "Download Data from Server";
            SyncUpRegistrationText = "Upload  Data To Server";

            Navigation = navigation;
        }

        private int localListing;

        public int LocalListing
        {
            get { return localListing; }
            set { SetProperty(ref localListing, value); }
        }

        private DateTime syncedUpload;

        public DateTime SyncedUpload
        {
            get { return syncedUpload; }
            set { SetProperty(ref syncedUpload, value); }
        }

        private DateTime syncedDownload;

        public DateTime SyncedDownload
        {
            get { return syncedDownload; }
            set { SetProperty(ref syncedDownload, value); }
        }

        #endregion Properties

        #region Registration Sync

        private string syncUpRegistrationText;

        public string SyncUpRegistrationText
        {
            get { return syncUpRegistrationText; }
            set { SetProperty(ref syncUpRegistrationText, value); }
        }

        private string syncDownRegistrationText;

        public string SyncDownRegistrationText
        {
            get { return syncDownRegistrationText; }
            set { SetProperty(ref syncDownRegistrationText, value); }
        }

        private string logRegistration;

        public string LogRegistration
        {
            get { return logRegistration; }
            set { SetProperty(ref logRegistration, value); }
        }

        private string messageRegistration;

        public string MessageRegistration
        {
            get { return messageRegistration; }
            set { SetProperty(ref messageRegistration, value); }
        }

        private ICommand syncUpRegistrationCommand;
        public ICommand SyncUpRegistrationCommand => syncUpRegistrationCommand ?? (syncUpRegistrationCommand = new Command(async () => await ExecuteSyncUpRegistrationAsync()));

        private async Task ExecuteSyncUpRegistrationAsync()
        {
            if (IsBusy)
            {
                return;
            }

            MessageRegistration = "Establishing Secure Connection... ...";
            SyncUpRegistrationText = "Working.";
            var uploadStatus = "";
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                    {
                        Title = "Sync Failed",
                        Message = "Uh Oh, It looks like you have gone offline. \n" +
                                  "Please check your internet connection and try again.",
                        Cancel = "OK"
                    });
                    return;
                }
                IsBusy = true;
                var enumerator = (Enumerator)App.Database.GetTableRow("Enumerator", "Id", Settings.EnumeratorId.ToString());

                MessageRegistration = "Uploading Sync Data  ...";

                SyncUpRegistrationText = "Uploading Data";

                var model = new LocalDeviceInfo()
                {
                    Version = CrossDeviceInfo.Current.Version,
                    AppBuild = CrossDeviceInfo.Current.AppBuild,
                    AppVersion = CrossDeviceInfo.Current.AppVersion,
                    DeviceName = CrossDeviceInfo.Current.DeviceName,
                    DeviceId = CrossDeviceInfo.Current.Id,
                    Idiom = CrossDeviceInfo.Current.Idiom,
                    IsDevice = CrossDeviceInfo.Current.IsDevice,
                    DeviceManufacturer = CrossDeviceInfo.Current.Manufacturer,
                    DeviceModel = CrossDeviceInfo.Current.Model,
                    Platform = CrossDeviceInfo.Current.Platform,
                    VersionNumber = CrossDeviceInfo.Current.VersionNumber.ToString()
                };

                var localdata = App.Database.GetTableRows<Registration>("Registration");
                int interviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Completed").Id;

                //MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message,
                //    new MessagingServiceAlert
                //    {
                //        Title = "interviewStatusId!",
                //        Message = interviewStatusId.ToString() +" Ready:"+ localdata.Count(i => i.InterviewStatusId == interviewStatusId),
                //        Cancel = "OK"
                //    });
                //MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message,
                //    new MessagingServiceAlert
                //    {
                //        Title = "interviewStatusId!",
                //        Message = localdata.First().InterviewStatusId.ToString() +" Status"+ localdata.First().StatusId,
                //        Cancel = "OK"
                //    });
                if (localdata.Any(i => i.InterviewStatusId == interviewStatusId))
                {
                    foreach (var item in localdata.Where(i => i.InterviewStatusId == interviewStatusId))
                    {
                        var household = item;

                        if (household.InterviewStatusId == interviewStatusId)
                        {
                            var members = App.Database.GetTableRows<RegistrationMember>("RegistrationMember", "RegistrationId", household.Id.ToString());
                            var programmes = App.Database.GetTableRows<RegistrationProgramme>("RegistrationProgramme", "RegistrationId", household.Id.ToString());
                            var disabilities = App.Database.GetTableRows<RegistrationMemberDisability>("RegistrationMemberDisability", "RegistrationId", household.Id.ToString());

                            household.SyncEnumeratorId = Settings.Current.EnumeratorId;
                            household.RegistrationMembers = members;
                            household.RegistrationMemberDisabilities = disabilities;
                            household.RegistrationProgrammes = programmes;

                            MessageRegistration = $"Processing Record Ref.#{household.Id} ... ";
                            var feedback = await client.PostRegistration(household, model);

                            switch (feedback.StatusId)
                            {
                                case null:
                                    Settings.Current.FirstRun = true;
                                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message,
                                        new MessagingServiceAlert
                                        {
                                            Title = "Sync Failed",
                                            Message = "Most probably, your session timed out ",
                                            Cancel = "OK"
                                        });
                                    return;

                                case 0:
                                    {
                                        App.Database.Delete<Registration>(household.Id);
                                        uploadStatus += "Record No. " + household.Id + " Uploaded\n";

                                        var response = "Data uploaded successfully.";

                                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message,
                                            new MessagingServiceAlert
                                            {
                                                Title = "Success!",
                                                Message = response,
                                                Cancel = "OK"
                                            });
                                        break;
                                    }

                                case -1:
                                    {
                                        if (feedback.Description.Contains("This Household has been submitted and Accepted"))
                                        {
                                            App.Database.Delete<Registration>(household.Id);

                                            uploadStatus += "Ref " + household.Id + " synced\n";
                                        }
                                        else
                                        {
                                            uploadStatus += "Ref " + household.Id + " " + feedback.Description + " \n";

                                            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message,
                                                new MessagingServiceAlert
                                                {
                                                    Title = "Sync Failed!",
                                                    Message = uploadStatus,
                                                    Cancel = "OK"
                                                });
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                }
                else
                {
                    // LogRegistration += $"{DateFormatter.ToSQLiteDateTimeString(DateTime.Now)} : No Cv Registered Households to Sync.\n \n";

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message,
                        new MessagingServiceAlert
                        {
                            Title = "Note",
                            Message = "No households ready for uploading.",
                            Cancel = "OK"
                        });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.InnerException?.Message);
                // MessageRegistration= ex.Message+"\n\n"+ ex.InnerException?.Message;
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Unable to Sync",
                    Message = "Data uploading failed. Please try again later.",
                    Cancel = "OK"
                });
            }
            finally
            {
                MessageRegistration = uploadStatus;
                SyncUpRegistrationText = "Upload Recorded Data";
                IsBusy = false;
                await Task.FromResult(0);
            }
        }

        private ICommand syncDownRegistrationCommand;

        public ICommand SyncDownRegistrationCommand => syncDownRegistrationCommand ?? (syncDownRegistrationCommand = new Command(async () => await ExecuteSyncDownRegistrationAsync()));

        private async Task ExecuteSyncDownRegistrationAsync()
        {
            if (IsBusy)
            {
                return;
            }

            MessageRegistration = "Establishing Secure Connection... ...";
            SyncDownRegistrationText = "Starting ...";
            try
            {
                IsBusy = true;
                EnumeratorCVResponse enumeratorResult = null;
                try
                {
                    var households = App.Database.GetTableRows<Registration>("Registration");

                    if (households.Any())
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                        {
                            Title = "Sync Failed",
                            Message = "Uh Oh, It looks like you have already downloaded data." +
                                      "Please delete or Fill and submit the downloaded data before downloading again",
                            Cancel = "OK"
                        });
                        return;
                    }
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                        {
                            Title = "Sync Failed",
                            Message = "Uh Oh, It looks like you have gone offline. Please check your internet connection to get the latest data and enable syncing data.",
                            Cancel = "OK"
                        });
                        return;
                    }

                    var enumerator = (Enumerator)App.Database.GetTableRow("Enumerator", "Id", Settings.EnumeratorId.ToString());

                    MessageRegistration = "Pulling data from the server... ...";
                    SyncDownRegistrationText = "Downloading Data";

                    enumeratorResult = await client.RegistrationListByEnumerator(enumerator.NationalIdNo, enumerator.PasswordHash, enumerator.Id.ToString());

                    if (enumeratorResult != null)
                    {
                        int InterviewStatusId = App.Database.SystemCodeDetailGetByCode("Interview Status", "Pending").Id;
                        int InterviewResultId = App.Database.SystemCodeDetailGetByCode("Interview Result", "Pending").Id;

                        if (enumeratorResult.Registrations.Any())
                        {
                            foreach (var item in enumeratorResult.Registrations)
                            {
                                item.InterviewStatusId = InterviewStatusId;
                                item.InterviewResultId = InterviewResultId;
                                item.DownloadDate = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                                App.Database.Create(item);

                                var member = new RegistrationMember
                                {
                                    IPRSed = true,
                                    MemberId = Guid.NewGuid().ToString(),
                                    RegistrationId = item.Id,
                                    FirstName = item.FirstName,
                                    MiddleName = item.MiddleName,
                                    Surname = item.Surname,
                                    DateOfBirth = DateTime.Parse(item.DateOfBirth),
                                    DateOfBirthDate = DateTime.Parse(item.DateOfBirth).ToString("O"),
                                    SexId = item.GenderId,
                                    IdentificationNumber = item.IdNumber,
                                    PhoneNumber = item.Phone,
                                    IdentificationDocumentTypeId = item.IdentificationFormId,
                                };
                                App.Database.AddOrUpdate(member);
                            }
                            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                            {
                                Title = "Success",
                                Message = "Data downloaded successfully",
                                Cancel = "OK"
                            });
                        }
                        else
                        {
                            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                            {
                                Title = "Success",
                                Message = "No new data found in the server.",
                                Cancel = "OK"
                            });
                        }

                        Settings.Current.LastSyncDown = DateTime.UtcNow;
                        Settings.Current.HasSyncedDataDownwards = true;
                    }
                }
                catch (Exception ex)
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                    {
                        Title = "Sync Complete",
                        Message = ex.Message,
                        Cancel = "OK"
                    });
                }
            }
            catch (Exception)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Unable to Sync",
                    Message = "The Sync failed. Please try again later.",
                    Cancel = "OK"
                });
            }
            finally
            {
                MessageRegistration = string.Empty;
                SyncDownRegistrationText = "Download Recorded Data";
                IsBusy = false;
                await Task.FromResult(0);
            }
        }

        #endregion Registration Sync
    }
}