namespace StajTakipUygulaması.Application.DTOs
{
    public class StajyerDto
    {
        public int ID { get; set; }

        // Öğrenci Bilgileri
        public string Universite { get; set; } = "";
        public string OgrenciNo { get; set; } = "";
        public string Bolum { get; set; } = "";
        public string Fakulte { get; set; } = "";
        public DateTime BaslamaYili { get; set; }
        public string Sinif { get; set; } = "";
        public bool PAU_ogrencisi_mi { get; set; }

        // Nüfus Bilgileri
        public string Ad { get; set; } = "";
        public string Soyad { get; set; } = "";
        public string TCKimlikNo { get; set; } = "";
        public DateTime DogumTarihi { get; set; }
        public string Cinsiyet { get; set; } = "";
        public string TelNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Adres { get; set; } = "";
    }
}
