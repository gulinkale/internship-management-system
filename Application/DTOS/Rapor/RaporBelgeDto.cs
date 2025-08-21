namespace StajTakipUygulaması.Application.DTOs
{
    public class RaporBelgeDto
    {
        public int Id { get; set; }
        public string BelgeAdi { get; set; } = "";
        public int BelgeTipiId { get; set; }
        public string? BelgeTipiAdi { get; set; }
        public string? Yol { get; set; }
    }
}
