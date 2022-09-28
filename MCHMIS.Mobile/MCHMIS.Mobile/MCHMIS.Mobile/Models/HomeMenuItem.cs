namespace MCHMIS.Mobile.Models
{
    public enum MenuItemType
    {
        HomePage,
       // HouseHoldListingPage,
       // CommunityValidationPage,
        RegistrationPage,
        RecertificationPage,
        SyncPage,
        LogoutPage
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
