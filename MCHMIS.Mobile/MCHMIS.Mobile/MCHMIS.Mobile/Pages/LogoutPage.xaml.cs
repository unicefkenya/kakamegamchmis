using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogoutPage : ContentPage
	{
	    private LoginViewModel vm;

	    public LogoutPage()
	    {
	        InitializeComponent();
	        BindingContext = vm = new LoginViewModel(Navigation);
	    }
    }
}