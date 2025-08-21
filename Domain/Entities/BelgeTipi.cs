using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel.DataAnnotations;
namespace StajTakipUygulaması.Domain.Entities
{
    public class BelgeTipi
    {
        public int ID { get; set; }

        public string Ad { get; set; }

        // İlişki: Bir belge tipi birden çok belgeye sahip olabilir
        public ICollection<Belge> Belgeler { get; set; } = new List<Belge>();
    }
}