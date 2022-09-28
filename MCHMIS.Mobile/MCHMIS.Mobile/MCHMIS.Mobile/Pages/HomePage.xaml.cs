using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
	    private HomeViewModel vm;

	    public HomePage()
	    {
	        InitializeComponent();

	        BindingContext = vm = new HomeViewModel(Navigation);
	    }
    }
}