using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Part1Cloud2B.Services
{
    public class AzureFileService
    {
        private readonly ShareClient _shareClient;

        public AzureFileService(string connectionString, string shareName)
        {
            _shareClient = new ShareClient("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net", shareName);
            _shareClient.CreateIfNotExists();
        }

        // Method to upload a file to Azure File Storage
        public async Task UploadFileAsync(string directoryName, string fileName, Stream fileStream)
        {
            try
            {
                var directoryClient = _shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(fileName);
                await fileClient.CreateAsync(fileStream.Length);
                await fileClient.UploadRangeAsync(new HttpRange(0, fileStream.Length), fileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        // Method to download a file from Azure File Storage
        public async Task<Stream> DownloadFileAsync(string directoryName, string fileName)
        {
            try
            {
                var directoryClient = _shareClient.GetDirectoryClient(directoryName);
                var fileClient = directoryClient.GetFileClient(fileName);

                ShareFileDownloadInfo download = await fileClient.DownloadAsync();

                MemoryStream memoryStream = new MemoryStream();
                await download.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset stream position to the beginning

                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                throw;
            }
        }

        // Method to delete a file from Azure File Storage
        public async Task DeleteFileAsync(string directoryName, string fileName)
        {
            try
            {
                var directoryClient = _shareClient.GetDirectoryClient(directoryName);
                var fileClient = directoryClient.GetFileClient(fileName);

                await fileClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                throw;
            }
        }
    }
}
