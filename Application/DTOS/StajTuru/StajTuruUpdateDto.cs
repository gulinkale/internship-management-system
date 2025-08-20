using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class StajTuruUpdateDto : StajTuruCreateDto
    {
        [Required]
        public int ID { get; set; }
    }
}
