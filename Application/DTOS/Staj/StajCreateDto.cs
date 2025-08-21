using System;
using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class StajCreateDto
    {
        [Required] public int StajyerID { get; set; }
        public string? Departman { get; set; }
        public string? SorumluID { get; set; }

        [Required] public DateTime BaslamaTarihi { get; set; }
        [Required] public DateTime BitisTarihi { get; set; }

        public string? Yetkiler { get; set; }

        [Required] public int StajTuruID { get; set; }
    }
}
