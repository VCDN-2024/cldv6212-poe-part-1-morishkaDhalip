using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Part1Cloud2B.Services
{
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net");
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }
    }
}
