using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UpSkillz.Models;  

public class BlobController : Controller
{
    private readonly BlobStorageService _blobStorageService;
    private readonly ILogger<BlobController> _logger;

    public BlobController(BlobStorageService blobStorageService, ILogger<BlobController> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var blobUrls = await _blobStorageService.GetAllBlobUrlsAsync();
        _logger.LogInformation("Retrieved {Count} blob URLs", blobUrls.Count);
        ViewBag.blobUrls = blobUrls;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(UploadModel model)
    {
        _logger.LogInformation("Uploading file to Blob Storage");
        if (model.File == null)
        {
            ModelState.AddModelError("File", "Please select a file.");
        }

        if (ModelState.IsValid)
        {
            var blobUrl = await _blobStorageService.UploadFileAsync(model.File);
            return RedirectToAction(nameof(Index));
        }

        var blobUrls = await _blobStorageService.GetAllBlobUrlsAsync();
        ViewBag.blobUrls = blobUrls;
        return View("Index");
    }

}