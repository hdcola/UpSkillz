using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace UpSkillz.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("BlobStorage__ConnectionString");
        var containerName = Environment.GetEnvironmentVariable("BlobStorage__ContainerName");
        _blobContainerClient = new BlobContainerClient(connectionString, containerName);
        _blobContainerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        Console.WriteLine("Uploading file to Blob Storage");
        var blobClient = _blobContainerClient.GetBlobClient(file.FileName);
        await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);
        Console.WriteLine("File uploaded to Blob Storage");
        return blobClient.Uri.ToString();
    }

    public async Task<List<string>> GetAllBlobUrlsAsync()
    {
        var blobUrls = new List<string>();
        await foreach (var blobItem in _blobContainerClient.GetBlobsAsync())
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
            blobUrls.Add(blobClient.Uri.ToString());
        }
        return blobUrls;
    }

}