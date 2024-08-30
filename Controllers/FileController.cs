using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Part1Cloud2B.Services;
using System;
using System.IO;
using System.Threading.Tasks;

public class FileController : Controller
{
    private readonly AzureFileService _fileService;

    public FileController(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultEndpointsProtocol=https;AccountName=st10033895storage;AccountKey=K51HJEDMhTHbMKFFnzGmjZH4Pfc/0Zzn/tgMzqe4l9z9b4oTqakTC0B2cj0WVgYS8GJMWDuGpV/s+AStpnwXMg==;EndpointSuffix=core.windows.net");
        _fileService = new AzureFileService(connectionString, "fileshare"); 
    }

    // GET: File/Upload
    public IActionResult Upload()
    {
        return View();
    }

    // POST: File/Upload
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
                    await _fileService.UploadFileAsync("my-directory", file.FileName, stream);
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

    // GET: File/Download/{fileName}
    public async Task<IActionResult> Download(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name cannot be empty.");
        }

        try
        {
            var stream = await _fileService.DownloadFileAsync("my-directory", fileName);
            return File(stream, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Error downloading file: {ex.Message}";
            return View("Error");
        }
    }

    // GET: File/Delete/{fileName}
    public async Task<IActionResult> Delete(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name cannot be empty.");
        }

        try
        {
            await _fileService.DeleteFileAsync("my-directory", fileName);
            ViewBag.Message = "File deleted successfully!";
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Error deleting file: {ex.Message}";
        }

        return View();
    }
}
