using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BasvuruReddetDto
    {
        [Required] public int ID { get; set; }
        [Required] public string RedNedeni { get; set; } = string.Empty;
    }
}
