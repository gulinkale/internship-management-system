namespace StajTakipUygulamasi.Application.DTOs
{
    public class BelgeCreateDto
    {
        public string BelgeAdı { get; set; } = "";
        public string? Açıklama { get; set; }
        public string Yolu { get; set; } = "";
        public int BelgeTipiID { get; set; }
        public int StajID { get; set; }
    }
}
