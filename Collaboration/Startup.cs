using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Collaboration.Startup))]
namespace Collaboration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
