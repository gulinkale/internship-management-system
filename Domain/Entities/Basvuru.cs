using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Domain.Entities
{
    public enum BasvuruDurumu { Beklemede = 0, Onaylandi = 1, Reddedildi = 2 }
    public class Basvuru
    {
        public int ID { get; set; }

        // Stajyer bilgileri (FK değil çünkü başvuru aşamasında bağımsız olabilir)
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string TelNo { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public string Universite { get; set; }
        public string Fakulte { get; set; }
        public string Bolum { get; set; }
        public string Sinif { get; set; }
        public DateTime BaslamaYili { get; set; }
        public string OgrenciNo { get; set; }

        // Staj bilgileri
        public string? Departman { get; set; }
        public string? SorumluID { get; set; }
        public string? Yetkiler { get; set; }

        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        // Staj Türü
        public int StajTuruID { get; set; }           // FK
        public StajTuru StajTuru { get; set; }        // Navigation

        // Başvuru zamanı
        public DateTime BasvuruTarihi { get; set; }

        // Belgeler
        public ICollection<BasvuruBelge> BasvuruBelgeleri { get; set; }






        // ... red ...
        public BasvuruDurumu Durum { get; set; } = BasvuruDurumu.Beklemede;

        public string? RedNedeni { get; set; }
        public DateTime? RedTarihi { get; set; }
        //public static object Durumu { get; internal set; }
    }
}
