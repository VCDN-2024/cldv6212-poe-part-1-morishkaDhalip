using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace Part1Cloud2B.Models
{
    public class CustomerProfile : ITableEntity
    {
        public int id { get; set; }
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;
      
    }

}
