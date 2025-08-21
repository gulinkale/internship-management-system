using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class BasvuruDto
    {
        public int ID { get; set; }

        // Kişisel
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string AdSoyad => $"{Ad} {Soyad}";
        public string TCKimlikNo { get; set; } = string.Empty;
        public DateTime DogumTarihi { get; set; }
        public string Cinsiyet { get; set; } = string.Empty;
        public string TelNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;

        // Eğitim
        public string Universite { get; set; } = string.Empty;
        public string Fakulte { get; set; } = string.Empty;
        public string Bolum { get; set; } = string.Empty;
        public string Sinif { get; set; } = string.Empty;
        public DateTime BaslamaYili { get; set; }
        public string OgrenciNo { get; set; } = string.Empty;

        // Staj
        public string? Departman { get; set; }
        public string? SorumluID { get; set; }
        public string? Yetkiler { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        // Staj Türü
        public int StajTuruID { get; set; }
        public string? StajTuruAdi { get; set; }

        // Başvuru
        public DateTime BasvuruTarihi { get; set; }

        // Durum
        public string Durum { get; set; } = string.Empty;  // Beklemede/Onaylandi/Reddedildi
        public string? RedNedeni { get; set; }
        public DateTime? RedTarihi { get; set; }

        // Belgeler
        public List<BasvuruBelgeListDto> Belgeler { get; set; } = new();
    }
}
