namespace ProductStore.Services;

public interface IImageStorage
{
    bool IsValid(IFormFile file, out string error);
    Task<string> SaveAsync(IFormFile file);
    void Delete(string? path);
}
