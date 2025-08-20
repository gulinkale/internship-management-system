namespace StajTakipUygulamasi.Application.DTOs
{
    public class BelgeDto
    {
        public int ID { get; set; }
        public string BelgeAdı { get; set; } = "";
        public string? Açıklama { get; set; }
        public string Yolu { get; set; } = "";     // /Belgeler/xxxx.ext

        public int BelgeTipiID { get; set; }
        public string? BelgeTipiAd { get; set; }   // join ile dolduracağız

        public int StajID { get; set; }
    }
}
