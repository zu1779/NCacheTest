using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NCacheTest.AspNetMvc5.Startup))]
namespace NCacheTest.AspNetMvc5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
