using Autofac;
using Autofac.Integration.WebApi;
using AzureLists.Library;
using AzureLists.Sql;
using System.Reflection;
using System.Web.Http;

namespace AzureLists.Api.App_Start
{
    public static class Dependencies
    {
        public static void RegisterDependencies(this HttpConfiguration configuration)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<SqlListRepository>().As<IListRepository>();
            builder.RegisterType<ListService>().AsSelf();

            IContainer container = builder.Build();

            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}