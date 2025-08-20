using System;

namespace Application.DTOs
{
    public class BasvuruListDto
    {
        public int ID { get; set; }
        public string AdSoyad { get; set; } = string.Empty; // Ad + Soyad
        public string? TCKimlikNo { get; set; }
        public string Durum { get; set; } = string.Empty;   // Beklemede/Onaylandi/Reddedildi
        public string? StajTuruAdi { get; set; }            // Nav: StajTuru.Ad
        public DateTime BasvuruTarihi { get; set; }
        public int BelgeSayisi { get; set; }
    }
}
