using MCHMIS.Mobile.Models;
using MCHMIS.Mobile.Pages;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        private List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.HomePage, Title="Home" },
                new HomeMenuItem {Id = MenuItemType.RegistrationPage, Title="Household Validation " },
                //new HomeMenuItem {Id = MenuItemType.RecertificationPage, Title="Monitoring Validation" },
                new HomeMenuItem {Id = MenuItemType.SyncPage, Title="Data Transfer" },
                new HomeMenuItem {Id = MenuItemType.LogoutPage, Title="Logout" },
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}