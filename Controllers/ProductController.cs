using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Part1Cloud2B.Models;
using Part1Cloud2B.Services;
using System;
using System.Threading.Tasks;

namespace Part1Cloud2B.Controllers
{
    public class ProductController : Controller
    {
        private readonly AzureTableService _tableService;
        private readonly AzureBlobService _blobService;

        public ProductController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net");
            _tableService = new AzureTableService(connectionString, "Products");
            _blobService = new AzureBlobService(connectionString);
        }

        // List all products
        public async Task<IActionResult> Index()
        {
            var products = await _tableService.GetAllEntitiesAsync<Product>();
            return View(products);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Upload image to Azure Blob Storage
                    var imageUrl = await _blobService.UploadBlobAsync(imageFile, "product-images");
                    product.ImageUrl = imageUrl;
                }

                product.PartitionKey = "Product";
                product.RowKey = Guid.NewGuid().ToString();
                await _tableService.AddEntityAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _tableService.GetEntityAsync<Product>("Product", id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile imageFile)
        {
            if (id != product.RowKey)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Upload new image to Azure Blob Storage and replace the old one
                    var imageUrl = await _blobService.UploadBlobAsync(imageFile, "product-images");
                    product.ImageUrl = imageUrl;
                }

                await _tableService.UpdateEntityAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product); // Return the view with the model to display validation errors if any
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _tableService.GetEntityAsync<Product>("Product", id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _tableService.GetEntityAsync<Product>("Product", id);
            if (product != null)
            {
                await _tableService.DeleteEntityAsync(product);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
