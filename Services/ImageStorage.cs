namespace ProductStore.Services;

public class ImageStorage(IWebHostEnvironment env) : IImageStorage
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxBytes = 2 * 1024 * 1024;

    public bool IsValid(IFormFile file, out string error)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (file.Length == 0)
            error = "The file is empty";
        else if (file.Length > MaxBytes)
            error = "The image must be 2 MB or smaller";
        else if (!AllowedExtensions.Contains(extension))
            error = "Only jpg, png and webp images are allowed";
        else
            error = "";

        return error == "";
    }

    // returns the path to store in the database, like /uploads/3f2a....jpg
    public async Task<string> SaveAsync(IFormFile file)
    {
        var folder = Path.Combine(env.WebRootPath, "uploads");
        Directory.CreateDirectory(folder);

        // never use the name the user sent: it can clash or contain a path
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName).ToLowerInvariant();

        await using var stream = File.Create(Path.Combine(folder, fileName));
        await file.CopyToAsync(stream);

        return "/uploads/" + fileName;
    }

    public void Delete(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var fullPath = Path.Combine(env.WebRootPath, path.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
