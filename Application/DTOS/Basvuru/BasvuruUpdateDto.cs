using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruUpdateDto : BasvuruCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
