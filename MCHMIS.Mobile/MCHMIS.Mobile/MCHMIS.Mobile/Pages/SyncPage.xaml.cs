using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SyncPage : ContentPage
	{
	    private SyncViewModel vm;

	    public SyncPage()
	    {
	        InitializeComponent();
	        BindingContext = vm = new SyncViewModel(Navigation);
	    }
    }
}