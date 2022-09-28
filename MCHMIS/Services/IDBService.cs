using System.Threading.Tasks;
using MCHMIS.Models;

namespace MCHMIS.Services
{
    public interface IDBService
    {
        int GetHealthFacilityId();

        int GetHealthFacilitySubCountyId();
        string GeneratePassword(int maxSize);
        void AuditTrail(AuditTrail auditTrail);
        Task<bool> IsGlobal();
    }
}