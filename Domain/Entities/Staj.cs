using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StajTakipUygulaması.Domain.Entities
{
    public class Staj
    {
        public int ID { get; set; } // PK
        public string? Departman { get; set; }
        public string? SorumluID { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string? Yetkiler { get; set; }

        public int StajyerID { get; set; } // FK
        [ValidateNever] public Stajyer? Stajyer { get; set; } // nav -> nullable + ValidateNever

        public ICollection<Belge> Belgeler { get; set; } = new List<Belge>();

        public int StajTuruID { get; set; }                 // FK (form buraya post eder)
        [ValidateNever] public StajTuru? StajTuru { get; set; } // nav -> nullable + ValidateNever
    }
}
