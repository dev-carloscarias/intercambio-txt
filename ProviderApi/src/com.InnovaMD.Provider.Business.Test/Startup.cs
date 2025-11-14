using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using com.InnovaMD.Provider.Business.Factories;
using com.InnovaMD.Provider.Business.Strategies.RequestingProvidier;
using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies;
using com.InnovaMD.Provider.Models.Common;
using com.InnovaMD.Provider.Models.Options;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Provider.Models.SystemConfiguration;
using com.InnovaMD.Utilities.Configuration.DatabaseProvider;
using com.InnovaMD.Utilities.Configuration.OptionModels;
using com.InnovaMD.Utilities.Dates;
using com.InnovaMD.Utilities.DistributedCache;
using com.InnovaMD.Utilities.Extensions.DistributedCache;
using com.InnovaMD.Utilities.Logging.OptionsModels;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace com.InnovaMD.Provider.Business.Test
{
    public class Startup
    {
        static SecretClient secretClient;

        public static IServiceProvider ServiceProvider
        {
            get; private set;
        }
        public static SecretClient KeyVaultClient
        {
            get; private set;
        }
        public static AzureServiceTokenProvider AzureServiceTokenProvider
        {
            get; private set;
        }
        private static readonly object lockObj = new object();

        public static int AccessCount = 0;

        private static bool IsStarting = false;

        public static void Start()
        {
            if (ServiceProvider == null)
            {
                lock (lockObj)
                {
                    if (!IsStarting)
                    {
                        Interlocked.Increment(ref AccessCount);

                        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("unittestsettings.json");

                        var configuration = builder.Build();

                        secretClient = GetSecretClient(configuration);

                        AddDatabaseConfiguration(configuration, builder);

                        configuration = builder.Build();

                        var services = new ServiceCollection();

                        ConfigureServices(services, configuration);

                        ConfigureLogger(services, configuration);

                        ServiceProvider = services.BuildServiceProvider();
                        IsStarting = true;
                    }
                }
            }
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //---- Configurations ----
            var connectionStringOptions = new ConnectionStringOptions()
            {
                ClinicalConsultation = secretClient.GetSecret(configuration.GetValue<string>("ConnectionStringOptions:AzureClinicalConsultationsConnectionStringSecretIdentifier")).Value.Value
            };

            services.AddSingleton<ConnectionStringOptions>(connectionStringOptions);
            var configurationOptions = new Utilities.Configuration.OptionModels.ConfigurationOptions() { DatabaseConnectionString = connectionStringOptions.ClinicalConsultation };
            services.AddSingleton(configurationOptions); //Required for the BaseSystemConfigurationOptionModel 
            services.AddScoped<ClinicalConsultationConfigurationModel>();
            services.AddScoped<ReportsConfigurationModel>();
            services.AddScoped<GlobalConfigurationModel>();
            services.Configure<EnvironmentConfigurationOptions>(configuration.GetSection(nameof(EnvironmentConfigurationOptions)));
            services.Configure<LoggerOptions>(configuration.GetSection(nameof(LoggerOptions)));


            //---- Components ----
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

            //---- Repositories ----
            services.AddScoped<IClinicalConsultationHistoryRepository, ClinicalConsultationHistoryRepository>();
            services.AddScoped<IReportsRepository, ReportsRepository>();
            services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
            services.AddScoped<ICreateClinicalConsultationRepository, CreateClinicalConsultationRepository>();
            services.AddScoped<IAdministrationGroupRequestingProviderRepository, AdministrationGroupRequestingProviderRepository>();
            services.AddScoped<IPayerRequestingProviderRepository, PayerRequestingProviderRepository>();
            services.AddScoped<IProviderRequestingProviderRepository, ProviderRequestingProviderRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();

            //---- Http Clients ----
            //services.AddSingleton<IIntegrationBusHttpClient>(new CustomHttpClient());

            DateExtensions.ConfigurationOptions = configurationOptions;

            services.AddScoped<ClinicalConsultationConfigurationModel>();

            services.AddDistributedMemoryCache();

            RegisterServerCache(services, configuration, secretClient);
        }

        private static void RegisterServerCache(IServiceCollection services, IConfiguration configuration, SecretClient secretClient)
        {
            var redisConnStringSecretIdentifier = configuration.GetValue<string>("ConnectionStringOptions:AzureRedisCacheConnectionStringSecretIdentifier");
            var redisConnString = secretClient.GetSecret(redisConnStringSecretIdentifier).Value.Value;
            var options = configuration.GetSection(nameof(CacheOptions)).Get<CacheOptions>();
            services.AddServerCache(redisConnString, options);
        }

        private static void AddDatabaseConfiguration(IConfigurationRoot configuration, IConfigurationBuilder builder)
        {
            var connectionString = secretClient.GetSecret(configuration.GetValue<string>("ConnectionStringOptions:AzureHCSSDBConnectionStringSecretIdentifier")).Value.Value;

            builder.AddDbConfiguration(options =>
            {
                options.ConnectionString = connectionString;
                options.ApplicationDomain = (int)ApplicationDomains.ProviderPortal;
            });
        }

        private static void ConfigureLogger(ServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddDebug();
            });
        }

        private static SecretClient GetSecretClient(IConfigurationRoot configuration)
        {
            return new SecretClient(new Uri(configuration.GetValue<string>("KeyVaultOptions:AzureVault")), new DefaultAzureCredential());
        }
    }
}
