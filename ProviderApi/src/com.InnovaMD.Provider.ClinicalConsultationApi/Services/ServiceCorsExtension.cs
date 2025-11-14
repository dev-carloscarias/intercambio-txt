using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceCorsExtension
    {
        public static void ConfigureServiceCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration
                .GetSection("OAuthOptions:AllowOrigins")
                .AsEnumerable()
                .Where(item => !string.IsNullOrEmpty(item.Value))
                .Select(item => item.Value)
                .ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy("provider.portalapi",
                    builder => builder.WithOrigins(origins)
                        .WithMethods("GET")
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }
    }
}
