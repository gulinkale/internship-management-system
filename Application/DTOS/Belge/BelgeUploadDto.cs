using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StajTakipUygulaması.Application.DTOs
{
    public class BelgeUploadDto
    {
        [FromForm] public IFormFile Dosya { get; set; }
        [FromForm] public int StajId { get; set; }
        [FromForm] public int BelgeTipId { get; set; }
        [FromForm] public string? Aciklama { get; set; }
    }
}
