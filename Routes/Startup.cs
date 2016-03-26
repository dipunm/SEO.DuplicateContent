using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Routes.Startup))]
namespace Routes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
