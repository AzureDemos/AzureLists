using System.Web.Http;
using Swashbuckle.Application;

namespace AzureLists.Api
{
    public static class SwaggerConfig
    {
        public static void RegisterSwagger(this HttpConfiguration configuration)
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            configuration
                .EnableSwagger(c => c.SingleApiVersion("v1", "AzureLists.Api"))
                .EnableSwaggerUi();
        }
    }
}
