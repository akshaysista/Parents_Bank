using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Parents_Bank.Startup))]
namespace Parents_Bank
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
