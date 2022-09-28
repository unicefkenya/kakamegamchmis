using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Interface;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using MCHMIS.Mobile.Pages;
using MCHMIS.Mobile.Services;
using MCHMIS.Mobile.ViewModels;
using FormsToolkit;
using Xamarin.Forms.DataGrid;

namespace MCHMIS.Mobile
{
    public partial class App : Application
    {
        public static App current;
        private bool registered;

        public App()
        {
            DataGridComponent.Init();
            Database = new DataStore();

            current = this;
            InitializeComponent();
            LocalBaseViewModel.Init();

            MainPage = new MainPage();
        }

        private static ILogger logger;
        public static DataStore Database { get; private set; }

        public static ILogger Logger => logger ?? (logger = DependencyService.Get<ILogger>());

        public static void GoToMainPage()
        {
            App.Current.MainPage = new MainPage();

            //   navigation = MainPage.Navigation;
#if FlyoutNavigation
            Current.MainPage = new RootMasterDetailPage();
#else

#endif
        }

        public static async Task<Position> GetCurrentPositionAsync()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 10;

                Settings.Current.Position = string.Empty;

                await locator.StartListeningAsync(TimeSpan.FromSeconds(10), 10).ConfigureAwait(false);
                position = await locator.GetLastKnownLocationAsync().ConfigureAwait(false);

                if (position != null)
                {
                    // got a cahched position, so let's use it.
                    return position;
                }

                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
                {
                    // not available or enabled
                    return null;
                }

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1), null, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

            if (position == null)
                return null;

            var output = string.Format(
                "Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                position.Timestamp,
                position.Latitude,
                position.Longitude,
                position.Altitude,
                position.AltitudeAccuracy,
                position.Accuracy,
                position.Heading,
                position.Speed);

            // Debug.WriteLine(output);

            return position;
        }

        public void SecondOnResume()
        {
            OnResume();
        }

        protected async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            // save current state and then set it
            var connected = Settings.Current.IsConnected;
            Settings.Current.IsConnected = e.IsConnected;
            if (connected && !e.IsConnected)
            {
                // we went offline, should alert the user and also update ui (done via settings)
                //var task = Current?.MainPage?.DisplayAlert(
                //    "Offline",
                //    "Uh Oh, It looks like you have gone offline. Please check your internet connection to get the latest data and enable syncing data.",
                //    "OK");

                DependencyService.Get<IToast>().SendToast("Uh Oh, It looks like you have gone offline. Please check your internet connection to get the latest data and enable syncing data.");
                //if (task != null)
                //    await task;
            }
        }

        protected override void OnResume()
        {
            if (registered)
                return;
            registered = true;

            // Handle when your app resumes
            Settings.Current.IsConnected = CrossConnectivity.Current.IsConnected;
            CrossConnectivity.Current.ConnectivityChanged += ConnectivityChanged;
            if (CrossGeolocator.IsSupported)
                CrossGeolocator.Current.PositionChanged += this.Position_Changed;

            // Handle when your app starts
            MessagingService.Current.Subscribe<MessagingServiceAlert>(
                MessageKeys.Message,
                async (m, info) =>
                {
                    if (Current?.MainPage != null)
                    {
                        var task = Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);

                        if (task == null)
                            return;

                        await task;
                    }

                    info?.OnCompleted?.Invoke();
                });
            MessagingService.Current.Subscribe<MessagingServiceAlert>(
                MessageKeys.Error,
                async (m, info) =>
                {
                    if (Current?.MainPage != null)
                    {
                        var task = Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);

                        if (task == null)
                            return;

                        await task;
                    }

                    info?.OnCompleted?.Invoke();
                });
            MessagingService.Current.Subscribe<MessagingServiceQuestion>(
                MessageKeys.Question,
                async (m, q) =>
                {
                    var task = Current?.MainPage?.DisplayAlert(q.Title, q.Question, q.Positive, q.Negative);
                    if (task == null)
                        return;
                    var result = await task;
                    q?.OnCompleted?.Invoke(result);
                });

            MessagingService.Current.Subscribe<MessagingServiceChoice>(
                MessageKeys.Choice,
                async (m, q) =>
                {
                    var task = Current?.MainPage?.DisplayActionSheet(q.Title, q.Cancel, q.Destruction, q.Items);
                    if (task == null)
                        return;
                    var result = await task;
                    q?.OnCompleted?.Invoke(result);
                });

            MessagingService.Current.Subscribe(
                MessageKeys.NavigateLogin,
                async m =>
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        ((MasterDetailPage)MainPage).IsPresented = false;
                    }

                    Page page = null;
                    if (Settings.Current.FirstRun && Device.RuntimePlatform == Device.Android)
                        page = new NavigationPage(new LoginPage());
                    else
                        page = new NavigationPage(new HomePage());

                    var nav = Current?.MainPage?.Navigation;
                    if (nav == null)
                        return;

                    await NavigationService.PushModalAsync(nav, page).ConfigureAwait(false);
                });
        }

        protected override void OnSleep()
        {
            if (!registered)
                return;

            registered = false;
            MessagingService.Current.Unsubscribe(MessageKeys.NavigateLogin);
            MessagingService.Current.Unsubscribe<MessagingServiceQuestion>(MessageKeys.Question);
            MessagingService.Current.Unsubscribe<MessagingServiceAlert>(MessageKeys.Error);
            MessagingService.Current.Unsubscribe<MessagingServiceAlert>(MessageKeys.Message);
            MessagingService.Current.Unsubscribe<MessagingServiceChoice>(MessageKeys.Choice);

            // Handle when your app sleeps
            CrossConnectivity.Current.ConnectivityChanged -= ConnectivityChanged;
        }

        protected override void OnStart()
        {
            // Settings.Current.FirstRun = true;
            // var deviceInfo = CrossDeviceInfo.Current;

            // Debug.WriteLine(deviceInfo);
            OnResume();
        }

        // Callback function for when GPS location changes
        private async void Position_Changed(object obj, PositionEventArgs e)
        {
            var position = e.Position;

            var lastKnownPosition = await CrossGeolocator.Current.GetLastKnownLocationAsync();
            if (e.Position != lastKnownPosition)
            {
                Settings.Current.Position = $"{position.Latitude},{position.Longitude}";
                Debug.WriteLine("Position changed: " + position.Latitude);
                Debug.WriteLine("Position changed: " + position.Longitude);

                // var task = Current?.MainPage?.DisplayAlert(
                // "Position Changed",
                // "Position changed: " + position.Latitude + "   " + position.Longitude,
                // "OK");
                // if (task != null)
                // await task;
            }

            // await this.StartListeningAsync().ConfigureAwait(false);
        }

        private async Task StartListeningAsync()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(
                TimeSpan.FromSeconds(5),
                10,
                true,
                new ListenerSettings
                {
                    ActivityType = ActivityType.AutomotiveNavigation,
                    AllowBackgroundUpdates = true,
                    DeferLocationUpdates = true,
                    DeferralDistanceMeters = 10,
                    DeferralTime = TimeSpan.FromSeconds(10),
                    ListenForSignificantChanges = true,
                    PauseLocationUpdatesAutomatically = false
                }).ConfigureAwait(false);

            CrossGeolocator.Current.PositionChanged += Position_Changed;
        }
    }
}