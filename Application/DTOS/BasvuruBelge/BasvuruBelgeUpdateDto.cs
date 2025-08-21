using StajTakipUygulaması.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace StajTakipUygulamasi.Application.DTOs.BasvuruBelge
{
    public class BasvuruBelgeUpdateDto : BasvuruBelgeCreateDto
    {
        [Required] public int ID { get; set; }
    }
}
