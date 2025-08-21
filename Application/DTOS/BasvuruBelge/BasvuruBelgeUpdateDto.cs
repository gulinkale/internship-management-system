using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BasvuruBelgeUpdateDto : BasvuruBelgeCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
