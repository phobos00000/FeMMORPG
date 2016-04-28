using System.Configuration;
using System.Web.Http;
using FeMMORPG.Synchronization;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace FeMMORPG.LoginServer
{
    internal static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            container.RegisterType<IPersistenceService, PersistenceService>(new HierarchicalLifetimeManager());

            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            container.RegisterInstance<IRepository<User>>(new MongoRepository<User>(connectionString));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}