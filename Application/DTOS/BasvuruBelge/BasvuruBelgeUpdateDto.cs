using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BasvuruBelgeUpdateDto : BasvuruBelgeCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
