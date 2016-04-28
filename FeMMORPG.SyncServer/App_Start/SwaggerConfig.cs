using System.Web.Hosting;
using System.Web.Http;
using Swashbuckle.Application;

namespace FeMMORPG.SyncServer
{
    internal static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "Synchronization Server");
                c.DescribeAllEnumsAsStrings(true);
                c.IncludeXmlComments(string.Format(@"{0}\bin\FeMMORPG.SyncServer.XML",
                    HostingEnvironment.ApplicationPhysicalPath));
            })
            .EnableSwaggerUi();
        }
    }
}