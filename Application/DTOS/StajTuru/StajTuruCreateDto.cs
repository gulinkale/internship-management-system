using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class StajTuruCreateDto
    {
        [Required, MinLength(2)]
        public string Ad { get; set; } = string.Empty;
    }
}
