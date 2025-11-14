using Microsoft.AspNetCore.Builder;

namespace com.InnovaMD.Provider.PortalApi.Configuration
{
    public static class ConfigureProtectionHeader
    {
        public static void ConfigureProtectionResponseHeader(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                await next();
            });
        }
    }
}
