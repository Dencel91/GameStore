
using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;

namespace ProductService.Services;

public class AzureFileService : IFileService
{
    private readonly BlobContainerClient _container;

    public AzureFileService(IConfiguration configuration)
    {
        var storageAccount = configuration["AzureStorage:StorageAccount"];
        var accessKey = Environment.GetEnvironmentVariable("AzureStorageAccessKey");
        var credential = new StorageSharedKeyCredential(storageAccount, accessKey);
        var blobUri = $"https://{storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);

        _container = blobServiceClient.GetBlobContainerClient("productimages");
    }

    public async Task<string> Upload(string fullPath, IFormFile file)
    {
        var blob = _container.GetBlobClient(fullPath);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = GetContentType(file.FileName)
        };

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, blobHttpHeaders);
            
        return blob.Uri.ToString();
    }

    private string GetContentType(string fileName)
    {
        var fileExtension = Path.GetExtension(fileName);
        return fileExtension.ToLower() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }

    public Task Delete(string fileName)
    {
        var blob = _container.GetBlobClient(fileName);
        return blob.DeleteIfExistsAsync();
    }
}
