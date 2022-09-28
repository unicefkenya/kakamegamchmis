using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationDetailPage : TabbedPage
    {
        public RegistrationDetailPage(int id, int child = 0)
        {
            InitializeComponent();
            BindingContext = new RegistrationDetailViewModel(Navigation, id);
            CurrentPage = Children[child];
            // this.CurrentPage = this.Children[navpage.Value];
        }
    }
}