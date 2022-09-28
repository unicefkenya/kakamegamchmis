using System.Threading.Tasks;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Models;

namespace MCHMIS.Mobile.Interface
{
    public interface IAppClient
    {
        Task<AccountResponse> ForgotPasswordAsync(string username);

        Task<AccountResponse> LoginAsync(string username, string password);

        Task<AuthKeyResponse> LoginAsync(string authKey);

        Task<EnumeratorLoginResponse> LoginEnumerator(string nationalIdNo, string pin, string id);

        Task<EnumeratorCVResponse> ValidationListByEnumerator(string nationalIdNo, string pin, string id);
        Task<EnumeratorCVResponse> RegistrationListByEnumerator(string nationalIdNo, string pin, string id);
        Task<EnumeratorCVResponse> RecertificationListByEnumerator(string nationalIdNo, string pin, string id);
         
        Task<ApiStatus> PostRegistration(Registration reg, LocalDeviceInfo deviceInfo);
         
        Task<ListingOptionsResponse> GetListingSettings(string nationalIdNo, string pin, string id);

        Task<ApiStatus> ForgotPin(string nationalIdNo, string emailAddress, string id);

        Task<ApiStatus> ChangePin(string currentPin, string newPin, string id);

        Task LogoutAsync();
    }
}