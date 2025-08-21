using System;
using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruCreateDto
    {
        // Kişisel
        [Required] public string Ad { get; set; } = string.Empty;
        [Required] public string Soyad { get; set; } = string.Empty;
        [Required] public string TCKimlikNo { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime DogumTarihi { get; set; }

        [Required] public string Cinsiyet { get; set; } = string.Empty;
        [Required] public string TelNo { get; set; } = string.Empty;

        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;

        // Eğitim
        [Required] public string Universite { get; set; } = string.Empty;
        [Required] public string Fakulte { get; set; } = string.Empty;
        [Required] public string Bolum { get; set; } = string.Empty;
        [Required] public string Sinif { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime BaslamaYili { get; set; } // entity ile birebir

        [Required] public string OgrenciNo { get; set; } = string.Empty;

        // Staj (opsiyonel)
        public string? Departman { get; set; }
        public string? SorumluID { get; set; }
        public string? Yetkiler { get; set; }

        [DataType(DataType.Date)] public DateTime? BaslamaTarihi { get; set; }
        [DataType(DataType.Date)] public DateTime? BitisTarihi { get; set; }

        // Staj Türü
        [Required] public int StajTuruID { get; set; }
    }
}
