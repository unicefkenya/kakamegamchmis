using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MCHMIS.Api.Startup))]

namespace MCHMIS.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
