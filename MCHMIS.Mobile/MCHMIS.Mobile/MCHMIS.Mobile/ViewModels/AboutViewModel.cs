using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class AboutViewModel : LocalBaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}