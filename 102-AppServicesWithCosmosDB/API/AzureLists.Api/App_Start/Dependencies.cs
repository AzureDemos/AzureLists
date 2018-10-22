using Autofac;
using Autofac.Integration.WebApi;
using AzureLists.CosmosDB;
using AzureLists.Library;
using System.Reflection;
using System.Web.Http;

namespace AzureLists.Api
{
    public static class Dependencies
    {
        public static void RegisterDependencies(this HttpConfiguration configuration)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ListService>().As<IListService>();
            builder.RegisterType<DocumentDBRepository<Library.List>>().As<IRepository<Library.List>>();

            IContainer container = builder.Build();

            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}