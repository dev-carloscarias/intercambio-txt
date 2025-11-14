//using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceVersioningCollection
    {
        public static void ConfigureServiceVersioning(this IServiceCollection services)
        {
            //Not yet supported for core 3.0
            //services.AddApiVersioning(o =>
            //{
            //    o.ReportApiVersions = true;
            //    o.ApiVersionReader = new HeaderApiVersionReader("api-version");
            //    o.DefaultApiVersion = new ApiVersion(1, 0);
            //    o.AssumeDefaultVersionWhenUnspecified = true;
            //});
        }
    }
}
