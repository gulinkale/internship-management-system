using System;

namespace Application.DTOs
{
    public class StajListDto
    {
        public int ID { get; set; }
        public int StajyerID { get; set; }
        public string? StajyerAdSoyad { get; set; }     // nav: Stajyer.Ad + Soyad

        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string? Departman { get; set; }

        public int StajTuruID { get; set; }
        public string? StajTuruAdi { get; set; }        // nav: StajTuru.Ad

        public bool AktifMi { get; set; }               // BitisTarihi >= Today
    }
}
