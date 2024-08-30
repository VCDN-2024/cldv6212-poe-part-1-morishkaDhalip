using Azure.Data.Tables;
using Azure;

namespace Part1Cloud2B.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Order";
        public string? RowKey { get; set; } // Order ID
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string ?ProductId { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;
    }

}
