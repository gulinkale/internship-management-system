using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Models
{
    public class Belge
    {
        public int ID { get; set; }

        public string BelgeAdı { get; set; }
        public string Açıklama { get; set; }
        public string Yolu { get; set; }

        // İlişkiler:
        public int BelgeTipiID { get; set; }
        public BelgeTipi BelgeTipi { get; set; } //FK

        public int StajID { get; set; } //FK
        public Staj Staj { get; set; } //Navigation 
        //public Stajyer stajyer { get; set; }
    }

}
