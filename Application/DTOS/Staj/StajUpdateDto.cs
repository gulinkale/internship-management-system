using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class StajUpdateDto : StajCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
