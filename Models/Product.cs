using Azure.Data.Tables;
using Azure;

namespace Part1Cloud2B.Models
{
    public class Product : ITableEntity
    {
        public string? PartitionKey { get; set; } = "Product";
        
        public string? RowKey { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? description { get; set; }
        public int? InventoryCount { get; set; }
        public string? ImageUrl { get; set; }  

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;
    }

}
