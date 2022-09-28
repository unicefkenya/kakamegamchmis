using System;
using System.Linq;
using Xamarin.Forms;
using MCHMIS.Mobile.Models;
using MCHMIS.Mobile.Services;
using MvvmHelpers;
using MCHMIS.Mobile.Interface;
using Plugin.DeviceInfo;
using System.Windows.Input;
using System.Threading.Tasks;
using MCHMIS.Mobile.Helpers;
using FluentValidation.Results;
using Plugin.Share;
using Plugin.Share.Abstractions;


namespace MCHMIS.Mobile.ViewModels
{
    public class LocalBaseViewModel : BaseViewModel
    {
       // public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>() ?? new MockDataStore();
        protected INavigation Navigation { get; }

        protected IToast Toast { get; } = DependencyService.Get<IToast>();
      //  protected ILogger Logger { get; } = DependencyService.Get<ILogger>();


      //  private ICommand launchBrowserCommand;

        public LocalBaseViewModel(INavigation navigation = null)
        {
            Navigation = navigation;

            AppVersion = $"Software Build {CrossDeviceInfo.Current.AppBuild} - Version. {CrossDeviceInfo.Current.AppVersion}. ";

            DisplayName = $"{Settings.Current.FirstName} {Settings.Current.LastName}";
        }

        private string _validateMessage;

        public string ValidateMessage
        {
            get
            {
                return _validateMessage;
            }
            set
            {
                SetProperty(ref this._validateMessage, value, "ValidateMessage");
            }
        }

        private string _appVersion;

        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
            set
            {
                SetProperty(ref this._appVersion, value, "AppVersion");
            }
        }

        private string _displayName;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                SetProperty(ref this._displayName, value, "DisplayName");
            }
        }
        protected string GetErrorListFromValidationResult(ValidationResult validationResult)
        {
            var errorList = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            return String.Join("\n", errorList.ToArray()); ;
        }

        public virtual void Subscribe()
        {
        }

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        public virtual void OnBackButtonPressed()
        {
        }

        public virtual void OnPopped(Page page)
        {
        }

        public string filter = string.Empty;

        public string Filter
        {
            get { return filter; }
            set { SetProperty(ref filter, value); }

        }


        public string message = string.Empty;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }



        //private bool isClean;

        //public bool IsClean
        //{
        //    get { return isClean; }
        //    set { SetProperty(ref isClean, value); }
        //}
        //private bool isNotClean;

        //public bool IsNotClean
        //{
        //    get => isNotClean;
        //    set
        //    {
        //        if (SetProperty(ref isNotClean, value))
        //            IsClean = !isNotClean;
        //    }
        //}
        //private bool isNotProcessed;

        //public bool IsNotProcessed
        //{
        //    get => isNotProcessed;
        //    set
        //    {
        //        if (SetProperty(ref isNotProcessed, value))
        //            IsProcessed = !isNotProcessed;
        //    }
        //}
        //private bool isProcessed;

        //public bool IsProcessed
        //{
        //    get { return isProcessed; }
        //    set { SetProperty(ref isProcessed, value); }
        //}

        //public ICommand LaunchBrowserCommand =>
        //    launchBrowserCommand ?? (launchBrowserCommand =
        //                                 new Command<string>(async (t) => await ExecuteLaunchBrowserAsync(t)));

        public Settings Settings
        {
            get
            {
                return Settings.Current;
            }
        }


        public static void Init(bool mock = true)
        {
            DependencyService.Register<IAppClient, AppClient>();
        }
        //private async Task ExecuteLaunchBrowserAsync(string arg)
        //{
        //    if (IsBusy)
        //        return;

        //    if (!arg.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        //        && !arg.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        //        arg = "http://" + arg;

        //    // Logger.Track(EvolveLoggerKeys.LaunchedBrowser, "Url", arg);
        //    var lower = arg.ToLowerInvariant();
        //    if (Device.RuntimePlatform == Device.iOS && lower.Contains("twitter.com"))
        //    {
        //        try
        //        {
        //            var id = arg.Substring(lower.LastIndexOf("/", StringComparison.Ordinal) + 1);
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    try
        //    {
        //        await CrossShare.Current.OpenBrowser(
        //            arg,
        //            new BrowserOptions
        //            {
        //                ChromeShowTitle = true,
        //                ChromeToolbarColor = new
        //                    ShareColor
        //                {
        //                    A = 255,
        //                    R = 118,
        //                    G = 53,
        //                    B = 235
        //                },
        //                UseSafariReaderMode = true,
        //                UseSafariWebViewController = true
        //            });
        //    }
        //    catch
        //    {
        //    }
        //}

    }
}
