using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Part1Cloud2B.Services
{
    public class AzureTableService
    {
        private readonly TableServiceClient _serviceClient;
        private TableClient _tableClient;

        public AzureTableService(string connectionString)
        {
            _serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net");
        }

        public AzureTableService(string connectionString, string tableName) : this(connectionString)
        {
            _tableClient = _serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        // Method to switch table within the same service client
        public void SetTable(string tableName)
        {
            _tableClient = _serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        // Method to retrieve a single entity from the table
        public async Task<T> GetEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Log or handle the 404 error when entity is not found
                Console.WriteLine($"Entity not found: PartitionKey={partitionKey}, RowKey={rowKey}");
                return null;
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                Console.WriteLine($"Error retrieving entity: {ex.Message}");
                throw;
            }
        }

        // Method to retrieve all entities from the table
        public async Task<List<T>> GetAllEntitiesAsync<T>() where T : class, ITableEntity, new()
        {
            var entities = new List<T>();

            try
            {
                await foreach (T entity in _tableClient.QueryAsync<T>())
                {
                    entities.Add(entity);
                }
            }
            catch (Exception ex)
            {
                // Handle potential exceptions and log the error
                Console.WriteLine($"Error retrieving entities: {ex.Message}");
                throw;
            }

            return entities;
        }

        // Method to add an entity to the table
        public async Task AddEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            try
            {
                await _tableClient.AddEntityAsync(entity);
            }
            catch (RequestFailedException ex)
            {
                // Handle error specific to Azure Table Storage
                Console.WriteLine($"Error adding entity: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine($"Error adding entity: {ex.Message}");
                throw;
            }
        }

        // Method to update an entity in the table
        public async Task UpdateEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            try
            {
                await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
            }
            catch (RequestFailedException ex)
            {
                // Handle error specific to Azure Table Storage
                Console.WriteLine($"Error updating entity: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine($"Error updating entity: {ex.Message}");
                throw;
            }
        }

        // Method to delete an entity from the table
        public async Task DeleteEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            try
            {
                await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
            }
            catch (RequestFailedException ex)
            {
                // Handle error specific to Azure Table Storage
                Console.WriteLine($"Error deleting entity: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine($"Error deleting entity: {ex.Message}");
                throw;
            }
        }
    }
}

