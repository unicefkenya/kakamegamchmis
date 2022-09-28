using System.Threading.Tasks;

namespace MCHMIS.Mobile.Interface
{
    public interface IPushNotifications
    {
        bool IsRegistered { get; }

        void OpenSettings();

        Task<bool> RegisterForNotifications();
    }
}