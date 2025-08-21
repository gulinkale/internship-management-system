namespace Application.DTOs
{
    public class BasvuruBelgeListDto
    {
        public int ID { get; set; }
        public string BelgeAdi { get; set; }
        public string? DosyaYolu { get; set; }
        public int BelgeTipiID { get; set; }
        public string? BelgeTipiAdi { get; set; }

        // Eklenmesi gereken alan
        public string? Aciklama { get; set; }
    }
}
