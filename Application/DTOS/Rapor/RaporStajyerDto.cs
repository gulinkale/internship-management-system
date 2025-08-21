using System.Collections.Generic;

namespace StajTakipUygulaması.Application.DTOs
{
    public class RaporStajyerDto
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = "";
        public string? TCKimlikNo { get; set; }
        public string? OgrenciNo { get; set; }
        public List<RaporStajDto> Stajlar { get; set; } = new();
    }
}
