
namespace StajTakipUygulaması.Domain.Entities
{
    public class Stajyer
    {
        public int ID { get; set; } //PK

        public ICollection<Staj> Stajlar { get; set; } = new List<Staj>();

        // Ogrenci Bilgileri
        public string Universite { get; set; }
        public string OgrenciNo { get; set; }
        public string Bolum { get; set; }
        public string Fakulte { get; set; }
        public DateTime BaslamaYili { get; set; }
        public string Sinif { get; set; } 
        public bool PAU_ogrencisi_mi { get; set; }

        // Nüfus Bilgileri (Nufus_ID yerine doğrudan)
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string TelNo { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
    }
}