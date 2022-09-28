using FluentValidation;
using FormsToolkit;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Interface;
using MCHMIS.Mobile.Models;
using MCHMIS.Mobile.Pages;
using MCHMIS.Mobile.Validators;
using Plugin.Connectivity;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class LoginViewModel : LocalBaseViewModel
    {
        private IAppClient client;

        public LoginViewModel(INavigation navigation) : base(navigation)
        {
            client = DependencyService.Get<IAppClient>();
            email = Settings.Current.Email;
            password = Settings.Current.Password;

            _validator = new LoginValidator();
        }

        private readonly IValidator _validator;

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

        private string pin;

        public string Pin
        {
            get { return pin; }
            set { SetProperty(ref pin, value); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string nationalId;

        public string NationalId
        {
            get { return nationalId; }
            set { SetProperty(ref nationalId, value); }
        }

        private string emailAddress;

        public string EmailAddress
        {
            get { return emailAddress; }
            set { SetProperty(ref emailAddress, value); }
        }

        private ICommand logoutCommand;

        public ICommand LogOutCommand =>
            logoutCommand ?? (logoutCommand = new Command(async () => await ExecuteLogoutAsync()));

        private async Task ExecuteLogoutAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                Message = "Establishing secure connection with the Main server ... ...";

                Settings.EnumeratorId = 0;
                Settings.AccessToken = null;
                Settings.LoggedIn = false;
                Settings.FirstRun = true;

                await Navigation.PopToRootAsync();
                await Navigation.PushAsync(new LoginPage());
                await Navigation.PushModalAsync(new LoginPage());
            }
            catch (Exception ex)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Unable to Log Out",
                    Message = " " + ex?.Message,
                    Cancel = "OK"
                });
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;
            }
            IsBusy = false;
        }

        private ICommand loginCommand;

        public ICommand LoginCommand =>
            loginCommand ?? (loginCommand = new Command(async () => await ExecuteLoginAsync()));

        private async Task ExecuteLoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            var validationResult = _validator.Validate(this);

            if (!validationResult.IsValid)
            {
                ValidateMessage = GetErrorListFromValidationResult(validationResult);
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Sign In Error",
                    Message = ValidateMessage,
                    Cancel = "OK"
                });
            }

            if (string.IsNullOrWhiteSpace(pin))
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Sign in Information",
                    Message = "We do need your PIN :-)",
                    Cancel = "OK"
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(nationalId))
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Sign in Information",
                    Message = "National ID Number is empty!",
                    Cancel = "OK"
                });
                return;
            }
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                    {
                        Title = "Login Failed",
                        Message = "It looks like you are offline. \n" +
                                  "Please check your internet connection and try again.",
                        Cancel = "OK"
                    });
                    return;
                }

                IsBusy = true;
                Message = "Establishing secure connection with the Main server ... ...";

                AuthKeyResponse result = null;
                EnumeratorLoginResponse enumeratorResult = null;

                //result = await client.LoginAsync(Constants.AuthKey);

                if (result?.Success != 0)
                {
                    Message = "Secure Connection Established...";

                    try
                    {
                        enumeratorResult = await client.LoginEnumerator(nationalId, pin, "");
                        if (enumeratorResult != null)
                        {
                            if (!string.IsNullOrEmpty(enumeratorResult.Error))
                            {
                                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                                {
                                    Title = "Unable to Sign in. Check your ID Number and Pin",
                                    Message = enumeratorResult.Error,
                                    Cancel = "OK"
                                });
                                return;
                            }
                            //else if (enumeratorResult.Wards == null)
                            //{
                            //    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                            //    {
                            //        Title = "Unable to Sign in.  Even though we were able to Log you In, It seems that y ",
                            //        Message = enumeratorResult.Error,
                            //        Cancel = "OK"
                            //    });
                            //    return;
                            //now Load into the Tables
                            //}
                            else
                            {
                                App.Database.AddOrUpdate(enumeratorResult.Enumerator);

                                foreach (var item in enumeratorResult.SystemCodes)
                                {
                                    App.Database.AddOrUpdate(item);
                                }

                                foreach (var item in enumeratorResult.SystemCodeDetails)
                                {
                                    App.Database.AddOrUpdate(item);
                                }

                                foreach (var item in enumeratorResult.Wards)
                                {
                                    App.Database.AddOrUpdate(item);
                                }

                                foreach (var item in enumeratorResult.Villages)
                                {
                                    App.Database.AddOrUpdate(item);
                                }
                                foreach (var item in enumeratorResult.CommunityAreas)
                                {
                                    App.Database.AddOrUpdate(item);
                                }
                                ////foreach (var item in enumeratorResult.Programmes)
                                ////{
                                ////    App.Database.AddOrUpdate(item);
                                ////}

                                ////foreach (var item in enumeratorResult.EnumeratorLocations)
                                ////{
                                ////    App.Database.AddOrUpdate(item);
                                ////}

                                Settings.FirstName = enumeratorResult?.Enumerator.FirstName ?? string.Empty;
                                Settings.LastName = enumeratorResult?.Enumerator.Surname ?? string.Empty;
                                Settings.Email = "chv@mchmis.go.ke";
                                Settings.EnumeratorId = enumeratorResult?.Enumerator.Id ?? 0;
                                Settings.Current.LastSyncDown = DateTime.UtcNow;
                                Settings.Current.HasSyncedDataDownwards = true;
                                Settings.FirstRun = false;
                                IsBusy = false;
                                // await Finish();
                                App.GoToMainPage();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                        {
                            Title = "Unable to Sign in.  ",
                            Message = ex.Message,
                            Cancel = "OK"
                        });
                        IsBusy = false;
                        return;
                    }
            }
                else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Unable to Sign in.",
                    Message = "Check your email and Password",
                    Cancel = "OK"
                });
            }
        }
            catch (Exception ex)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Unable to Sign in",
                    Message = ex.Message,
                    Cancel = "OK"
                });
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;
            }
        }

        private ICommand signupCommand;

        public ICommand SignupCommand =>
            signupCommand ?? (signupCommand = new Command(async () => await ExecuteSignupAsync()));

        private async Task ExecuteSignupAsync()
        {
            await CrossShare.Current.OpenBrowser("https://www.inuajamii.go.ke:2010/public/", new BrowserOptions
            {
                ChromeShowTitle = true,
                ChromeToolbarColor = new ShareColor
                {
                    A = 255,
                    R = 118,
                    G = 53,
                    B = 235
                },
                UseSafariReaderMode = false,
                UseSafariWebViewController = true
            });
        }

        private ICommand recoverPinCommand;

        public ICommand RecoverPinCommand =>
            recoverPinCommand ?? (recoverPinCommand = new Command(async () => await ExecuteRecoverPinPageAsync()));

        private async Task ExecuteRecoverPinPageAsync()
        {
            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
            {
                Title = "Password Recovery Instructions",
                Message = "Kindly Contact Programme Team to Send you a new PIN to Access the MCH MIS Data Collection Tool \n" +
                          "You may be required to answer several questions to prove authenticity and ownership of the Account. ",
                Cancel = "OK"
            });
            return;
        }

        private async Task Finish()
        {
            if (Device.RuntimePlatform == Device.iOS && Settings.FirstRun)
            {
                var push = DependencyService.Get<IPushNotifications>();
                if (push != null)
                {
                    await push.RegisterForNotifications();
                }

                await Navigation.PopModalAsync();
            }
            else
            {
                App.GoToMainPage();
            }
        }
    }
}