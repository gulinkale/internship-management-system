namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IFileStorage
    {
        // Kaydeder ve /Belgeler/abcd.pdf gibi WEB'den erişilebilir göreli yol döndürür
        Task<string> SaveAsync(Stream content, string originalFileName, string subFolder);
    }
}
