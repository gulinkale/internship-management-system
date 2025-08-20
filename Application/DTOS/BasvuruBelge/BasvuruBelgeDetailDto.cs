namespace Application.DTOs
{
    public class BasvuruBelgeDetailDto
    {
        public int ID { get; set; }
        public string BelgeAdi { get; set; } = string.Empty;   // Entity: BelgeAdı
        public string? Aciklama { get; set; }                  // Entity: Açıklama
        public string? DosyaYolu { get; set; }                 // Entity: Yolu

        public int BelgeTipiID { get; set; }
        public string? BelgeTipiAdi { get; set; }              // Nav: BelgeTipi.Ad

        public int BasvuruID { get; set; }
        public string? BasvuruAdSoyad { get; set; }            // Nav: Basvuru.Ad + Soyad
    }
}
