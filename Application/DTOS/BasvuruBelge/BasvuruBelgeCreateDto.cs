using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruBelgeCreateDto
    {
        [Required] public string BelgeAdi { get; set; } = string.Empty; // Entity: BelgeAdı
        public string? Aciklama { get; set; }                           // Entity: Açıklama
        public string? DosyaYolu { get; set; }                          // Entity: Yolu

        [Required] public int BelgeTipiID { get; set; }
        [Required] public int BasvuruID { get; set; }

        // Not: Eğer dosya upload edeceksen Controller katmanında ayrıca IFormFile almanı öneririm.
    }
}
