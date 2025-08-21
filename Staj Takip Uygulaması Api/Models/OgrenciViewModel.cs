using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StajTakipUygulaması.Domain.Entities; // bunu da ekle en üste

namespace StajTakipUygulaması.Models
{
    public class OgrenciViewModel
    {

        public Stajyer Stajyer { get; set; } = new Stajyer();


        public Staj Staj { get; set; } = new Staj();

        public IFormFile? OgrenciBelgesi { get; set; }
        public IFormFile? Transkript { get; set; }
        public IFormFile? BasvuruFormu { get; set; }
        public IFormFile? Taahutname { get; set; }
        public IFormFile? Referans { get; set; }

        public int StajTuruID { get; set; }

        // PAÜ Dışı Öğrenciye Özel Alanlar (opsiyonel)
        public DateTime? DogumTarihi { get; set; }  // nullable olmalı
        public string? YokBarkodNo { get; set; }    // nullable olmalı

        public IFormFile? Fotograf { get; set; }

    }
}
