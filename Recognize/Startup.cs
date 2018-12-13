using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Recognize.Startup))]
namespace Recognize
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
