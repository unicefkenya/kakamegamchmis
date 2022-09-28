using System;

namespace MCHMIS.Interfaces
{
    public interface IDatabaseService : IDisposable
    {
        string GetComputerName(string clientIP);

        string GetExternalIP();
    }
}