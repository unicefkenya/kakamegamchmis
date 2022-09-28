using System;
using MCHMIS.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MCHMIS.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationMemberPage : ContentPage
    {
        RegistrationMemberViewModel ViewModel => vm ?? (vm = BindingContext as RegistrationMemberViewModel);
        private RegistrationMemberViewModel vm;

        public RegistrationMemberPage(int hhId, string memberId)
        {
            InitializeComponent();
            BindingContext = new RegistrationMemberViewModel(Navigation, hhId, memberId);


            var Year = DateTime.Now.Year + 1;
           
            var minDate = Year - 1900;
            var mincgOPCTyr = new DateTime(Year - minDate, 1, 1);

            DateOfBirth.SetValue(DatePicker.MaximumDateProperty, DateTime.Now);
            DateOfBirth.SetValue(DatePicker.MinimumDateProperty, mincgOPCTyr);

               }

    }
}