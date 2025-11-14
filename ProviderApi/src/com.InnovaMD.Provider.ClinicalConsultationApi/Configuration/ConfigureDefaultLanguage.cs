using com.InnovaMD.Provider.Models.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace com.InnovaMD.Provider.PortalApi.Configuration
{
    public static class ConfigureDefaultLanguage
    {
        public static void SetDefaultLanguage(this IApplicationBuilder app)
        {
            var environmentConfigurationOptions = app.ApplicationServices.GetService<IOptions<EnvironmentConfigurationOptions>>().Value;
            var culture = environmentConfigurationOptions.ResourceLanguage;
            if (environmentConfigurationOptions.ResourceLanguage.Contains("PR"))
            {
                //en-PR is not a valid culture.  this workaround helps the application use the default resx file.
                culture = "es-US";
            }
            var cultureInfo = new CultureInfo(culture);

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
