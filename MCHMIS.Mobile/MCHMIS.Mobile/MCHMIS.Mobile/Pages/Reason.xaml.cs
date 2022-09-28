using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Reason : PopupPage
    {
        public Reason(Registration registration)
        {
            InitializeComponent();
            BindingContext = new CannotFindHHReasonViewModel(Navigation, registration);
        }

        public async void btnCloseClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}