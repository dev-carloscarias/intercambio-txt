using Azure.Security.KeyVault.Secrets;
using com.InnovaMD.Provider.Models.Log;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Logging;
using com.InnovaMD.Utilities.Logging.OptionsModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;

namespace com.InnovaMD.Provider.PortalApi.Configuration
{
    public static class ConfigureLoggerExtension
    {
        public static void ConfigureLoggers(this ILoggingBuilder loggingBuilder, IConfiguration configuration, IServiceCollection services, SecretClient keyVaultClient)
        {
            var serviceProvider = services.BuildServiceProvider();

            var loggerOptions = serviceProvider.GetService<IOptions<LoggerOptions>>().Value;

            if (configuration.GetValue<bool>("Environment:IsPublish"))
            {
                var azureEventHubsConnStringSecretIdentifier = configuration.GetValue<string>("ConnectionStringOptions:AzureEventHubsConnStringSecretIdentifier");
                var azureEventHubsConnString = keyVaultClient.GetSecretAsync(azureEventHubsConnStringSecretIdentifier).Result.Value.Value;

                loggerOptions.AzureEventHubsConnString = azureEventHubsConnString;
                loggerOptions.DefaultFunctionality = configuration.GetValue<string>("LoggerOptions:DefaultFunctionality");
                loggerOptions.ApplicationDomain = nameof(ApplicationDomains.ProviderPortal);

                loggingBuilder.AddAzureLogger(
                      appliationCategoryPrefix: "com.InnovaMD"
                    , options: loggerOptions
                    , serviceProvider: serviceProvider
                    , typeof(MyExceptionEvent));

                loggingBuilder.AddApplicationAzureLogger(
                      appliationCategoryPrefix: "com.InnovaMD"
                    , options: loggerOptions
                    , serviceProvider: serviceProvider
                    , typeof(MyAuditEvent)
                    , typeof(MyExceptionEvent));
            }
            else
            {
                loggingBuilder.AddDebug();
            }
        }
    }
}
