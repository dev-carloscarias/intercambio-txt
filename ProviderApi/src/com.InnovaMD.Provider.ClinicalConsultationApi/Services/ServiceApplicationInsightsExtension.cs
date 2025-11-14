using com.InnovaMD.Utilities.Logging.OptionsModels;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceApplicationInsightsExtension
    {
        public static void ConfigureApplicationInsightsTelemetry(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
        {
            if (configuration.GetValue<bool>("Environment:IsPublish")
                && configuration.GetValue<bool>("ApplicationInsights:Enable"))
            {
                var aiOptions = new ApplicationInsightsServiceOptions()
                {
                    InstrumentationKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey"),
                    EnableAdaptiveSampling = configuration.GetValue<bool>("ApplicationInsights:EnableAdaptiveSampling"),
                    EnableAuthenticationTrackingJavaScript = configuration.GetValue<bool>("ApplicationInsights:EnableAuthenticationTrackingJavaScript"),
                    DeveloperMode = configuration.GetValue<bool>("ApplicationInsights:DeveloperMode"),
                    EnableDebugLogger = configuration.GetValue<bool>("ApplicationInsights:EnableDebugLogger"),
                    EnableHeartbeat = configuration.GetValue<bool>("ApplicationInsights:EnableHeartbeat"),
                    EnableDependencyTrackingTelemetryModule = configuration.GetValue<bool>("ApplicationInsights:EnableDependencyTrackingTelemetryModule"),
                };

                services.AddApplicationInsightsTelemetry(aiOptions);
                services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
                {
                    module.EnableSqlCommandTextInstrumentation = configuration.GetValue<bool>("ApplicationInsights:EnableSqlCommandTextInstrumentation");
                    o = aiOptions;
                });

                var logLevel = (LogLevel)configuration.GetValue<int>("ApplicationInsights:LogLevel");
                builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(l => l >= logLevel);
            }
        }
    }
}
