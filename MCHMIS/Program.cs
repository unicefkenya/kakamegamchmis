using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MCHMIS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromHours(1); })
                .UseIISIntegration()
                .UseSetting(WebHostDefaults.DetailedErrorsKey,"true")
                .Build();
    }
}
