using Azure.Data.Tables;
using Azure;

namespace Part1Cloud2B.Models
{
    public class OrderItem : ITableEntity
    {
        public string? PartitionKey { get; set; } // Order ID
        public string? RowKey { get; set; } // OrderItem ID
        public string? ProductId { get; set; }
        public int ?Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice => Quantity * UnitPrice;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;
    }

}
