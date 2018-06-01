using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApiInsight.Administrator.Startup))]
namespace WebApiInsight.Administrator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
