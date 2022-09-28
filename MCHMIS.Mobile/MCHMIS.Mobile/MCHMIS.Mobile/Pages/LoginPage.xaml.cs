using System;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
	    private LoginViewModel vm;

	    public LoginPage()
	    {
	        InitializeComponent();

	        BindingContext = vm = new LoginViewModel(Navigation);
	    }

	    protected override bool OnBackButtonPressed()
	    {
	        return Settings.Current.FirstRun || base.OnBackButtonPressed();
	    }
	    private async void OnTermsClicked(object sender, EventArgs e)
	    {
	        Device.OpenUri(new Uri(Constants.BaseSiteAddress + "/public/terms-and-conditions/"));
	    }
    }
}