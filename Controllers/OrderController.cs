using Microsoft.AspNetCore.Mvc;
using Part1Cloud2B.Models;
using Part1Cloud2B.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

public class OrderController : Controller
{
    private readonly AzureTableService _orderTableService;
    private readonly AzureTableService _productTableService;
    private readonly AzureQueueService _queueService;

    public OrderController(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("AzureStorageConnectionString");
        _orderTableService = new AzureTableService(connectionString, "Orders");  // Service for Orders table
        _productTableService = new AzureTableService(connectionString, "Products");  // Service for Products table
        _queueService = new AzureQueueService(connectionString, "order-queue");  // Service for Queue Storage
    }

    // GET: Order/Index
   
    public async Task<IActionResult> Index()
    {
        // Retrieve all orders from Azure Table Storage
        var orders = await _orderTableService.GetAllEntitiesAsync<Order>();

        return View(orders);
    }


    // GET: Order/Create
    public async Task<IActionResult> Create()
    {
        // Retrieve all products from Azure Table Storage
        var products = await _productTableService.GetAllEntitiesAsync<Product>();

        if (products == null || !products.Any())
        {
            ModelState.AddModelError("", "No products available.");
            return View(new OrderViewModel());
        }

        // Populate the OrderViewModel with the product list
        var model = new OrderViewModel
        {
            Products = products.Select(p => new SelectListItem
            {
                Value = p.RowKey, // ProductId
                Text = $"{p.Name} - {p.Price}" // Display name and price
            }).ToList()
        };

        return View(model);
    }

    // POST: Order/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Retrieve the existing product based on the selected ProductId
            var product = await _productTableService.GetEntityAsync<Product>("Product", model.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Ensure the order is created using the existing product and does not add a new product
            if (product.InventoryCount < model.Quantity)
            {
                ModelState.AddModelError("", "Not enough inventory available.");
                return View(model);
            }

            // Deduct the ordered quantity from the inventory
            product.InventoryCount -= model.Quantity;
            await _productTableService.UpdateEntityAsync(product);

            // Create a new order using the existing product details
            var order = new Order
            {
                PartitionKey = "Order",
                RowKey = Guid.NewGuid().ToString(), // Unique ID for the order
                CustomerName = $"{model.CustomerName} {model.CustomerSurname}",
                CustomerEmail = model.CustomerEmail,
                ProductId = model.ProductId, // Link the order to the existing product by ProductId
                Quantity = model.Quantity,
                TotalPrice = product.Price.Value * model.Quantity,
                Timestamp = DateTimeOffset.UtcNow
            };

            // Add the order to the Orders table
            await _orderTableService.AddEntityAsync(order);

            // Send a message to the queue indicating that an order has been placed
            string queueMessage = $"Order {order.RowKey} for product {product.Name} has been placed by {order.CustomerName}.";
            await _queueService.SendMessageAsync(queueMessage);

            // Redirect to the confirmation page
            return RedirectToAction("Confirmation", new { orderId = order.RowKey });
        }

        // Re-populate product list for the drop-down if validation fails
        var products = await _productTableService.GetAllEntitiesAsync<Product>();
        model.Products = products.Select(p => new SelectListItem
        {
            Value = p.RowKey,
            Text = $"{p.Name} - {p.Price.Value.ToString("C")}"
        }).ToList();

        return View(model);
    }

    // GET: Order/Confirmation
    public async Task<IActionResult> Confirmation(string orderId)
    {
        if (string.IsNullOrEmpty(orderId))
        {
            return NotFound(); // Return 404 if orderId is missing
        }

        var order = await _orderTableService.GetEntityAsync<Order>("Order", orderId);

        if (order == null)
        {
            return NotFound(); // Return 404 if the order is not found
        }

        // Fetch the product details to get the product name
        var product = await _productTableService.GetEntityAsync<Product>("Product", order.ProductId);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        // Pass the product name to the view using ViewBag or update the model
        ViewBag.ProductName = product.Name;

        return View(order); // Pass the order to the view
    }

}



/*private async Task SaveCustomerProfileAsync(OrderViewModel model)
{
    var customerProfile = new CustomerProfile
    {
        PartitionKey = "Customer",
        RowKey = Guid.NewGuid().ToString(),
        Name = model.CustomerName,
        Surname = model.CustomerSurname,
        Email = model.CustomerEmail,

    };

    await _tableService.AddEntityAsync(customerProfile);
}
*/







