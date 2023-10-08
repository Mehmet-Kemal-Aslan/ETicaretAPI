using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstractions.Storage.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace ETicaretAPI.Infrastructure.Services.Storage.Azure
{
    public class AzureStorage : Services.Storage.Storage, IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;
        private readonly string _connectionString;

        public AzureStorage()
        {
            _connectionString = "DefaultEndpointsProtocol=https;AccountName=minieticaretmka;AccountKey=KAls/ZF4SAxk6jKRSR7Xp935BP5mazT46+3+7gsB0xTln/9Qjw02phtepPD4z2y58yQNkYyR7Yzz+AStsfVKhQ==;EndpointSuffix=core.windows.net";
            _blobServiceClient = new BlobServiceClient(_connectionString);
            //_blobServiceClient = "DefaultEndpointsProtocol=https;AccountName=minieticaretmka;AccountKey=KAls/ZF4SAxk6jKRSR7Xp935BP5mazT46+3+7gsB0xTln/9Qjw02phtepPD4z2y58yQNkYyR7Yzz+AStsfVKhQ==;EndpointSuffix=core.windows.net";
        }
        public async Task DeleteAsync(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        public List<string> GetFiles(string containerName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Select(x => x.Name).ToList();
        }

        public bool HasFile(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Any(x => x.Name == fileName);
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string containerName, IFormFileCollection files)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<(string fileName, string pathOrContainerName)> data = new();
            foreach(IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(containerName, file.Name, HasFile);

                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNewName);
                await blobClient.UploadAsync(file.OpenReadStream());
                data.Add((fileNewName, containerName));
            }

            return data;
        }
    }
}
