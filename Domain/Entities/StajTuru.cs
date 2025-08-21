using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel.DataAnnotations;
using StajTakipUygulaması.Models;
namespace StajTakipUygulaması.Domain.Entities
{
    public class StajTuru
    {
        public int ID { get; set; }
        public string Ad { get; set; }

        public ICollection<Staj> Stajlar { get; set; } = new List<Staj>();
    }
}
