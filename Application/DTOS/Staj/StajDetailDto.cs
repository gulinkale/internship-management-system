using System;

namespace StajTakipUygulaması.Application.DTOs
{
    public class StajDetailDto
    {
        public int ID { get; set; }
        public int StajyerID { get; set; }
        public string? StajyerAdSoyad { get; set; }

        public string? Departman { get; set; }
        public string? SorumluID { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string? Yetkiler { get; set; }

        public int StajTuruID { get; set; }
        public string? StajTuruAdi { get; set; }

        public bool AktifMi { get; set; }
    }
}
