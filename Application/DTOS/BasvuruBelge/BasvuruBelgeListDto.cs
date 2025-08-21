namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruBelgeListDto
    {
        public int ID { get; set; }
        public string BelgeAdi { get; set; } = string.Empty;   // Entity: BelgeAdı
        public string? BelgeTipiAdi { get; set; }              // Nav: BelgeTipi.Ad
        public string? DosyaYolu { get; set; }                 // Entity: Yolu
        public int BasvuruID { get; set; }
    }
}
