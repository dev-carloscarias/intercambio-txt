using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Configuration.DatabaseProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;


namespace com.InnovaMD.Provider.PortalApi.Configuration
{
    public static class ConfigureConnection
    {
        public static (IConfiguration, SecretClient, DefaultAzureCredential) GetConfiguration(this IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();

            var configuration = builder.Build();
            var defaultAzureCredential = AzureCredentialFactory.ConfigureCredentials(configuration);
            var keyVaultClient = configuration.GetSecretClient(defaultAzureCredential);

            var connectionStringSecretIdentifier = configuration.GetValue<string>("ConnectionStringOptions:AzureClinicalConsultationsConnectionStringSecretIdentifier");
            var connectionString = keyVaultClient.GetSecret(connectionStringSecretIdentifier).Value.Value;

            builder.AddDbConfiguration(options =>
            {
                options.ConnectionString = connectionString;
                options.ApplicationDomain = (int)ApplicationDomains.ProviderPortal; 
            });

            configuration = builder.Build();

            return (configuration, keyVaultClient, defaultAzureCredential);
        }

        public static SecretClient GetSecretClient(this IConfiguration configuration, DefaultAzureCredential defaultAzureCredential)
        {
            
            var client = new SecretClient(new Uri(configuration.GetValue<string>("KeyVaultOptions:AzureVault")), defaultAzureCredential);
            return client;
        }
    }
}
