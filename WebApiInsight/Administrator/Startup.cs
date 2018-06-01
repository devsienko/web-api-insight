using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Administrator.Startup))]
namespace Administrator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
