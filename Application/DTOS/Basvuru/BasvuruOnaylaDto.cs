using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruOnaylaDto
    {
        [Required] public int ID { get; set; }
        // İstersen OnayNotu, OnayTarihi gibi alanlar da eklenebilir.
    }
}
