using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;

namespace Part1Cloud2B.Services
{
    public class AzureQueueService
    {
        private readonly QueueClient _queueClient;

        public AzureQueueService(string connectionString, string Messages)
        {
            _queueClient = new QueueClient("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net", Messages);
            _queueClient.CreateIfNotExists();
        }

        // Method to send a message to the queue
        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_queueClient.Exists())
                {
                    await _queueClient.SendMessageAsync(message);
                    Console.WriteLine($"Message sent: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }

        // Method to receive a message from the queue
        public async Task<QueueMessage[]> ReceiveMessagesAsync(int maxMessages = 1)
        {
            try
            {
                if (_queueClient.Exists())
                {
                    return await _queueClient.ReceiveMessagesAsync(maxMessages);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
                throw;
            }

            return Array.Empty<QueueMessage>();
        }

        // Method to delete a message from the queue
        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            try
            {
                if (_queueClient.Exists())
                {
                    await _queueClient.DeleteMessageAsync(messageId, popReceipt);
                    Console.WriteLine($"Message deleted: {messageId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting message: {ex.Message}");
                throw;
            }
        }
    }
}
