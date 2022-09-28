using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationEditPage : ContentPage
    {
        public RegistrationEditPage (int id)
        {
            InitializeComponent();
            BindingContext = new RegistrationEditViewModel(Navigation, id);

        }
    }
}