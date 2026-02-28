namespace StajTakipUygulaması.Application.DTOs
{
    public class BelgeUploadRequest
    {
        public Stream Content { get; set; } = Stream.Null; // dosya içeriği
        public string OriginalFileName { get; set; } = ""; // orijinal ad (uzantı için)
        public int BelgeTipiID { get; set; }
        public int StajID { get; set; }
    }
}
