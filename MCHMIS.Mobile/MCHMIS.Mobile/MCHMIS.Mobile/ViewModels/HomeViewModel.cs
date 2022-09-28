using MCHMIS.Mobile.Interface;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    
    public class HomeViewModel : LocalBaseViewModel
    {
        private IAppClient client;

        public HomeViewModel(INavigation navigation) : base(navigation)
        {
            client = DependencyService.Get<IAppClient>(); 
        }

    }
}
