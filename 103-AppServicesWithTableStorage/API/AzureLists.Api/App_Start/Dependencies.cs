using Autofac;
using Autofac.Integration.WebApi;
using AzureLists.Library;
using AzureLists.TableStorage;
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

            builder.RegisterType<TableStorageRepository>().AsSelf();
            builder.RegisterType<TableStorageListService>().As<IListService>();
            var fakeAuthenticatedUser = UserCredentials.GetFirstUserFirstPartion();
            builder.Register(c => fakeAuthenticatedUser).As<UserCredentials>();
            IContainer container = builder.Build();

            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}