using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegistrationListPage : TabbedPage
	{
		RegistrationListViewModel ViewModel => vm ?? (vm = BindingContext as RegistrationListViewModel);
		private RegistrationListViewModel vm;
		//private bool showFavs, showPast, showAllCategories;
		//private string filteredCategories;
		private ToolbarItem filterItem;
		private string loggedIn;

		public RegistrationListPage()
		{
			InitializeComponent();
			loggedIn = Settings.Current.Email;

			BindingContext = vm = new RegistrationListViewModel(Navigation);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is RegistrationListViewModel bindingContext)
			{
				bindingContext.OnAppearing();
				vm.DownloadedRegistrations.ReplaceRange(bindingContext.GetDownloadedRegistrations());
				vm.OngoingRegistrations.ReplaceRange(bindingContext.GetOngoingRegistrations());
				vm.CompleteRegistrations.ReplaceRange(bindingContext.GetCompleteRegistrations());
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			vm.DownloadedRegistrations.Clear();
			vm.OngoingRegistrations.Clear();
			vm.CompleteRegistrations.Clear();

		}
	}
}