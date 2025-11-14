using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace com.InnovaMD.Provider.PortalApi.Configuration
{
    public static class ConfigureForwardedHeaders
    {
        public static void ConfigureUseFowardedHeaders(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor,
                ForwardLimit = 2
            });
        }
    }
}
