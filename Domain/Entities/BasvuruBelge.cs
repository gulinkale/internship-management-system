using Microsoft.AspNetCore.Mvc.ModelBinding;
using StajTakipUygulaması.Models;
using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Models
{
    public class BasvuruBelge
    {
        public int ID { get; set; }

        public string BelgeAdı { get; set; }
        public string Açıklama { get; set; }
        public string Yolu { get; set; }

        // İlişkiler:
        public int BelgeTipiID { get; set; }
        public BelgeTipi BelgeTipi { get; set; } //FK

        public int BasvuruID { get; set; } //FK
        public Basvuru Basvuru { get; set; } //Navigation 
        //public Stajyer stajyer { get; set; }
    }

}
