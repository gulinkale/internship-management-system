using StajTakipUygulamasi.Application.Interfaces;

public class FileSystemFileStorage : IFileStorage
{
    private readonly string _webRoot;

    // DI'dan düz string webRootPath alıyoruz (IWebHostEnvironment YOK)
    public FileSystemFileStorage(string webRootPath)
    {
        _webRoot = string.IsNullOrWhiteSpace(webRootPath)
            ? Path.Combine(AppContext.BaseDirectory, "wwwroot")
            : webRootPath;

        Directory.CreateDirectory(_webRoot);
    }

    public async Task<string> SaveAsync(Stream content, string originalFileName, string subFolder)
    {
        var folder = Path.Combine(_webRoot, (subFolder ?? "").Trim('/', '\\'));
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(originalFileName);
        var name = $"{Guid.NewGuid():N}{ext}";
        var physical = Path.Combine(folder, name);

        using var fs = new FileStream(physical, FileMode.Create);
        await content.CopyToAsync(fs);

        return "/" + Path.Combine(subFolder ?? "", name).Replace("\\", "/");
    }
}
