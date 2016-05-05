using System.Data.Entity;
using System.Web.Http;
using FeMMORPG.Data;
using FeMMORPG.LoginServer.App_Start;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace FeMMORPG.LoginServer
{
    internal static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IUserUnitOfWork, UserUnitOfWork>(new HierarchicalLifetimeManager());
            container.RegisterType<DbContext, UserDbContext>(new HierarchicalLifetimeManager());
            container.RegisterInstance(AutoMapperConfig.GetMapperConfiguration().CreateMapper());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}