using AzureLists.Api.App_Start;
using System.Web.Http;
using System.Web.Http.Cors;
namespace AzureLists.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.RegisterDependencies();
            config.RegisterSwagger();

            var cors = new EnableCorsAttribute("*","*","*");
            config.EnableCors(cors);
        }
    }
}
