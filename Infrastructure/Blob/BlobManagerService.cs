using System;
using System.IO;
using System.Threading.Tasks;
using Application.Interfaces;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using BlobType = Domain.Enums.BlobType;

namespace Infrastructure.Blob
{
    public class BlobManagerService : IBlobManagerService
    {
        private BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ClientSecretCredential _clientSecretCredential;

        public BlobManagerService(IConfiguration config)
        {
            _clientSecretCredential = new ClientSecretCredential(
                config.GetValue<string>("AzureAD:TenantId"),
                config.GetValue<string>("AzureAD:ClientId"),
                config.GetValue<string>("AzureAD:ClientSecret")
            );
            
            _blobServiceClient = new BlobServiceClient(
                new Uri(config.GetValue<string>("BlobStorage:BlobUrl")), 
                _clientSecretCredential);
            
            _blobContainerClient = _blobServiceClient
                .GetBlobContainerClient(config.GetValue<string>("BlobStorage:ContainerName"));
        }

        public async Task<string> UploadToBlobStorageAsync(
            Guid guid,
            string fileNameWithExtension, 
            Stream file, 
            BlobType type)
        {
            await CreateClient();

            file.Position = 0;
            
            var blobName = String.Concat(
                type.ToString(), 
                '/', 
                guid.ToString(), 
                "-", 
                fileNameWithExtension);
            
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(file, true);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> RemoveFromBlobStorageAsync(string url)
        {
            try
            {
                await CreateClient();
            
                var uri = new Uri(url);
            
                string containerName = uri.Segments[1];
                string blobName = string.Concat(uri.Segments[2],uri.Segments[3]);
            
                var blobClient = _blobContainerClient.GetBlobClient(blobName);
                
                await blobClient.DeleteIfExistsAsync();
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        private async Task CreateClient()
        {
            await _blobContainerClient.CreateIfNotExistsAsync();
        }
    }
}
