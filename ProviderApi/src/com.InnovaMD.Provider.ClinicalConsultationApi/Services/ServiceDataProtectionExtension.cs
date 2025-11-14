using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using System;
using Azure.Security.KeyVault.Keys;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceDataProtectionExtension
    {
        public static void ConfigureDataProtection(this IServiceCollection services, IConfiguration configuration, SecretClient keyVaultClient, DefaultAzureCredential defaultAzureCredential)
        {
            // Data Protection Configuration
            var azureStorageAccountKeysConnectionString = configuration.GetValue<string>("DataProtectionOptions:AzureStorageAccountKeysConnectionString");
            var storageAccountConnectionString = keyVaultClient.GetSecretAsync(azureStorageAccountKeysConnectionString).Result.Value.Value;
            var dataProtectionKeysContainerNames = configuration.GetValue<string>("DataProtectionOptions:AzureStorageAccountContainer");

            // Protect Keys With Azure KeyVault
            var keyVault = new KeyClient(new Uri(configuration.GetValue<string>("KeyVaultOptions:AzureVault")), defaultAzureCredential);
            var azureProtectKeysWithAzureKeyVaultConnectionString = configuration.GetValue<string>("KeyVaultOptions:AzureProtectKeysWithAzureKeyVaultConnectionString");
            var protectKeysWithAzureKeyVaultConnectionString = keyVault.GetKeyAsync(azureProtectKeysWithAzureKeyVaultConnectionString).Result.Value.Id;

            // Flag Credentials
            BlobServiceClient blobServiceClient;
            if (configuration.GetValue<bool>("Environment:IsIdentity"))
            {
                // Identity
                var accountUri = new Uri($"https://{configuration.GetValue<string>("DataProtectionOptions:AzureStorageAccountName")}.blob.core.windows.net/");
                blobServiceClient = new BlobServiceClient(accountUri, defaultAzureCredential);
            }
            else
            {
                //Connection String
                blobServiceClient = new BlobServiceClient(storageAccountConnectionString);
            }
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(dataProtectionKeysContainerNames);

            // optional - provision the container automatically
            blobContainerClient.CreateIfNotExists();

            var blobClient = blobContainerClient.GetBlobClient("keys.xml");

            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(blobClient)
                .ProtectKeysWithAzureKeyVault(protectKeysWithAzureKeyVaultConnectionString, defaultAzureCredential)
                .SetApplicationName(configuration.GetValue<string>("DataProtectionOptions:ApplicationName"))
                .DisableAutomaticKeyGeneration(); //have a read-only view of the key ring

        }
    }
}
