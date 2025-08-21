using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruReddetDto
    {
        [Required] public int ID { get; set; }
        [Required] public string RedNedeni { get; set; } = string.Empty;
    }
}
