using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulaması.Application.DTOs
{
    public class StajTuruUpdateDto : StajTuruCreateDto
    {
        [Required]
        public int ID { get; set; }
    }
}
