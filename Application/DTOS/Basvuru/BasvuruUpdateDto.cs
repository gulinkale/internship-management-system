using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BasvuruUpdateDto : BasvuruCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
