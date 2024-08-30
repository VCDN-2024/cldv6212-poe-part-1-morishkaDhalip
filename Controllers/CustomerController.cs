using Microsoft.AspNetCore.Mvc;
using Part1Cloud2B.Models;
using Part1Cloud2B.Services;

namespace Part1Cloud2B.Controllers
{
    public class CustomerController : Controller
    {

        
        
            private readonly AzureTableService _tableService;

            public CustomerController(IConfiguration configuration)
            {
                // Retrieve the Azure Storage connection string from the configuration
                string connectionString = configuration.GetConnectionString("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net");
                _tableService = new AzureTableService(connectionString, "CustomerProfiles");
            }

            // Action to list all customers
            public async Task<IActionResult> CustomerView()
            {
                var customers = await _tableService.GetAllEntitiesAsync<CustomerProfile>();
                return View(customers);
            }

            // Action to add a new customer
            [HttpPost]
            public async Task<IActionResult> AddCustomer(CustomerProfile customer)
            {
                customer.PartitionKey = "Customer";
                customer.RowKey = Guid.NewGuid().ToString();
                await _tableService.AddEntityAsync(customer);
                return RedirectToAction("CustomerCreate");
            }

            
        }
    }
    

