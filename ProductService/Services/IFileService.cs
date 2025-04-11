namespace ProductService.Services;

public interface IFileService
{
    Task<string> Upload(string fullPath, IFormFile file);

    Task Delete(string fileName);
}
