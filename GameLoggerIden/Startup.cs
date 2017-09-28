using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GameLoggerIden.Startup))]
namespace GameLoggerIden
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}