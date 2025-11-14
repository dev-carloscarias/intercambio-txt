using Azure.Security.KeyVault.Secrets;
using com.InnovaMD.Provider.PortalApi.Configuration;
using com.InnovaMD.Provider.PortalApi.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Schema;
using com.InnovaMD.Utilities.Logging.OptionsModels;
using Azure.Identity;
using com.InnovaMD.Utilities.Configuration.DatabaseProvider;
using com.InnovaMD.Utilities.Extensions.DistributedCache;

namespace com.InnovaMD.Provider.PortalApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly SecretClient _secretClient;
        private readonly DefaultAzureCredential _defaultAzureCredential;


        public Startup(IWebHostEnvironment environment)
        {
            (_configuration, _secretClient, _defaultAzureCredential) = environment.GetConfiguration();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.RegisterServices(_configuration, _secretClient);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.ConfigureServiceAuthentication();
            services.ConfigureServiceAuthorization(_configuration);
            services.ConfigureServiceCors(_configuration);
            services.ConfigureDataProtection(_configuration, _secretClient, _defaultAzureCredential);

            services.ConfigureApplicationInsightsTelemetry(_configuration, builder);

            RegisterNewtonsoftJsonSchemaLicence();
            ConfigureHangfire();

            AddEventLog(services);

            if (_configuration.GetValue<bool>("LoggerOptions:Enable"))
            {
                services.AddLogging(builder =>
                {
                    builder.ConfigureLoggers(_configuration, services, _secretClient);
                });
            }

            services.ConfigureServiceVersioning();
        }

        private void AddEventLog(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerOptions = serviceProvider.GetService<IOptions<LoggerOptions>>().Value;

            services.AddLogging(builder => builder.AddEventLog(new EventLogSettings()
            {
                Filter = (category, logLevel) => logLevel >= (LogLevel)loggerOptions.LogLevel,
                SourceName = _configuration.GetValue<bool>("Environment:IsAzure") ? "Application" : loggerOptions.EventLogSourceName
            }));
        }

        private void RegisterNewtonsoftJsonSchemaLicence()
        {
            // Register NewtonsoftJsonSchema License
            var license = _configuration.GetValue<string>("NewtonsoftJsonSchemaOptions:License");

            if (!string.IsNullOrEmpty(license))
            {
                License.RegisterLicense(license);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.ConfigureUseFowardedHeaders();
            app.ConfigureProtectionResponseHeader();

            if (!_configuration.GetValue<bool>("Environment:IsPublish"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.ApplicationServices.UseConfigurationCache();
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("provider.portalapi");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureHangfire()
        {
            var hangfireConnStringSecretIdentifier = _configuration.GetValue<string>("ConnectionStringOptions:HangfireConnectionStringSecretIdentifier");
            var hangfireConnString = _secretClient.GetSecret(hangfireConnStringSecretIdentifier).Value.Value;

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(new Func<SqlConnection>(() =>
                {
                    var conn = new SqlConnection(hangfireConnString);
                    return conn;
                }), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });
        }
    }
}
