using MCHMIS.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.ViewModels;
using FormsToolkit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        private bool isRunning = false;
        

        private LocalBaseViewModel vm;
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = vm = new LocalBaseViewModel(Navigation);
            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.HomePage, (NavigationPage)Detail);

        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.HomePage:
                        MenuPages.Add(id, new NavigationPage(new HomePage()));
                        break;
                    case (int)MenuItemType.RegistrationPage:
                        MenuPages.Add(id, new NavigationPage(new RegistrationListPage()));
                        break;


                    case (int)MenuItemType.SyncPage:
                        MenuPages.Add(id, new NavigationPage(new SyncPage()));
                        break;
                    case (int)MenuItemType.LogoutPage:
                        MenuPages.Add(id, new NavigationPage(new LogoutPage()));
                        break;

                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
            if (newPage == null)
                return;

            // if we are on the same tab and pressed it again.
            if (Detail == newPage)
            {
                await newPage.Navigation.PopToRootAsync();
            }

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Settings.Current.FirstRun)
            {
                MessagingService.Current.SendMessage(MessageKeys.NavigateLogin);
            }

            isRunning = true;

      
        }


    }
}