using StajTakipUygulaması.Application.Interfaces;

namespace StajTakipUygulaması.Infrastructure.Services
{
    public class FileSystemFileStorage : IFileStorage
    {
        private readonly string _webRoot;

        // IWebHostEnvironment.WebRootPath kullanılacak
        public FileSystemFileStorage(string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentNullException(nameof(webRootPath));

            _webRoot = webRootPath;
            Directory.CreateDirectory(_webRoot);
        }

        public async Task<string> SaveAsync(Stream content, string originalFileName, string subFolder = "Belgeler")
{
    var folder = Path.Combine(_webRoot, subFolder ?? "");
    Directory.CreateDirectory(folder);

    var ext = Path.GetExtension(originalFileName);
    var name = $"{Guid.NewGuid():N}{ext}";
    var physical = Path.Combine(folder, name);

    using var fs = new FileStream(physical, FileMode.Create);
    await content.CopyToAsync(fs);

    // ✅ Burayı değiştirdik
    var relative = "/" + Path.Combine(subFolder ?? "", name).Replace("\\", "/");
    var absolute = "http://localhost:5235" + relative; // API portun

    return absolute;
}

    }
}
