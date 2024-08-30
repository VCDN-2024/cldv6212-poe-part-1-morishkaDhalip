using Microsoft.AspNetCore.Mvc;
using Part1Cloud2B.Services;

namespace Part1Cloud2B.Controllers
{
    public class MediaController : Controller
    {
        private readonly AzureBlobService _blobService;

        public MediaController(IConfiguration configuration)
        {
            string connectionString = configuration["DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net"];
            _blobService = new AzureBlobService(connectionString);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var imageUrl = await _blobService.UploadBlobAsync(file, "images");
            ViewBag.ImageUrl = imageUrl;
            return View();
        }
    }
}
