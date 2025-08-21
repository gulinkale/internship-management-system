using System;
using System.Collections.Generic;

namespace StajTakipUygulaması.Application.DTOs
{
    public class RaporStajDto
    {
        public int Id { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public List<RaporBelgeDto> Belgeler { get; set; } = new();
    }
}
