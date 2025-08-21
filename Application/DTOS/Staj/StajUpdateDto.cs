using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class StajUpdateDto : StajCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
