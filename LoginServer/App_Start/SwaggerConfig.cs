using System.Web.Hosting;
using System.Web.Http;
using Swashbuckle.Application;

namespace FeMMORPG.LoginServer
{
    internal static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "Login Server");
                c.DescribeAllEnumsAsStrings(true);
                c.IncludeXmlComments(string.Format(@"{0}\bin\FeMMORPG.LoginServer.XML",
                    HostingEnvironment.ApplicationPhysicalPath));
            })
            .EnableSwaggerUi();
        }
    }
}
