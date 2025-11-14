using Azure.Security.KeyVault.Secrets;
using com.InnovaMD.Provider.Business;
using com.InnovaMD.Provider.Business.Factories;
using com.InnovaMD.Provider.Business.Strategies.RequestingProvidier;
using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies;
using com.InnovaMD.Provider.Models.Common;
using com.InnovaMD.Provider.Models.SystemConfiguration;
using com.InnovaMD.Utilities.Configuration.OptionModels;
using com.InnovaMD.Utilities.Dates;
using com.InnovaMD.Utilities.DistributedCache;
using com.InnovaMD.Utilities.Extensions.DistributedCache;
using com.InnovaMD.Utilities.Logging.OptionsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration, SecretClient keyVaultClient)
        {
            services.AddOptions();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton(keyVaultClient);

            RegisterConfigurations(services, configuration, keyVaultClient);

            RegisterRepositories(services, configuration);

            RegisterBusinessComponents(services, configuration);

            RegisterHttpClients(services, configuration);

            RegisterServerCache(services, configuration, keyVaultClient);

            return services;
        }

        private static void RegisterConfigurations(IServiceCollection services, IConfiguration configuration, SecretClient keyVaultClient)
        {
            services.Configure<ConnectionStringOptions>(configuration.GetSection(nameof(ConnectionStringOptions)));
            services.Configure<OAuthOptions>(configuration.GetSection(nameof(OAuthOptions)));
            services.Configure<LoggerOptions>(configuration.GetSection(nameof(LoggerOptions)));


            var connectionStringOptions = new ConnectionStringOptions()
            {
                ClinicalConsultation = keyVaultClient.GetSecretAsync(configuration.GetValue<string>("ConnectionStringOptions:AzureClinicalConsultationsConnectionStringSecretIdentifier")).Result.Value.Value,
            };

            services.AddSingleton(connectionStringOptions);

            //Example of databasee configuration loaded at sartup
            //services.Configure<EnvironmentConfigurationOptions>(configuration.GetSection(nameof(EnvironmentConfigurationOptions)));

            var configurationOptions = new ConfigurationOptions()
            {
                DatabaseConnectionString = connectionStringOptions.ClinicalConsultation,
            };
            services.AddSingleton(configurationOptions);

            services.AddScoped<ClinicalConsultationConfigurationModel>();
            services.AddScoped<ReportsConfigurationModel>();
            services.AddScoped<GlobalConfigurationModel>();

            //Uncomment this line when DateExtensions are available
            DateExtensions.ConfigurationOptions = configurationOptions;
        }

        private static void RegisterRepositories(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClinicalConsultationHistoryRepository, ClinicalConsultationHistoryRepository>();
            services.AddScoped<IReportsRepository, ReportsRepository>();
            services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
            services.AddScoped<ICreateClinicalConsultationRepository, CreateClinicalConsultationRepository>();
            services.AddScoped<IAdministrationGroupRequestingProviderRepository, AdministrationGroupRequestingProviderRepository>();
            services.AddScoped<IPayerRequestingProviderRepository, PayerRequestingProviderRepository>();
            services.AddScoped<IProviderRequestingProviderRepository, ProviderRequestingProviderRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
        }

        private static void RegisterBusinessComponents(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILogComponent, LogComponent>();
            services.AddScoped<IClinicalConsultationHistoryComponent, ClinicalConsultationHistoryComponent>();
            services.AddScoped<IDocumentComponent, DocumentComponent>();
            services.AddScoped<ICreateClinicalConsultationComponent, CreateClinicalConsultationComponent>();
            services.AddScoped<IBeneficiaryComponent, BeneficiaryComponent>();
            services.AddScoped<SearchRequestingProviderStrategyFactory>();

            var strategiesRequesting = AppDomain.CurrentDomain.GetAssemblies()
             .SelectMany(assembly => assembly.GetTypes())
             .Where(type => typeof(ISearchRequestingProviderStrategy).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var strategy in strategiesRequesting)
            {
                services.AddScoped(typeof(ISearchRequestingProviderStrategy), strategy);
            }
        }

        private static void RegisterHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            //TODO - Example of how to make HTTP requests using IHttpClientFactory - typed client.  Remove if not necesary.
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1#typed-clients
            //var reportUrl = configuration.GetValue<string>("UriOptions:ReportWrapper");
            //services.AddHttpClient<DummyService>(c =>
            //{
            //    c.BaseAddress = new Uri(reportUrl);
            //});
        }

        private static void RegisterServerCache(IServiceCollection services, IConfiguration configuration, SecretClient secretClient)
        {
            var redisConnStringSecretIdentifier = configuration.GetValue<string>("ConnectionStringOptions:AzureRedisCacheConnectionStringSecretIdentifier");
            var redisConnString = secretClient.GetSecret(redisConnStringSecretIdentifier).Value.Value;
            var options = configuration.GetSection(nameof(CacheOptions)).Get<CacheOptions>();
            services.AddServerCache(redisConnString, options);
        }
    }
}
