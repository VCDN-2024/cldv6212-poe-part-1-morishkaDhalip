using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Part1Cloud2B.Services;
using System;
using System.IO;
using System.Threading.Tasks;

public class CustomerServiceController : Controller
{
    private readonly AzureFileService _fileService;

    public CustomerServiceController(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net");
        _fileService = new AzureFileService(connectionString, "customer-service-files"); // Create a share named "customer-service-files"
    }

    // GET: CustomerService/Upload
    public IActionResult Upload()
    {
        return View();
    }

    // POST: CustomerService/Upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await _fileService.UploadFileAsync("reviews-complaints", file.FileName, stream);
                }
                ViewBag.Message = "File uploaded successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error uploading file: {ex.Message}";
            }
        }
        else
        {
            ViewBag.Message = "Please select a file.";
        }

        return View();
    }

    
}
