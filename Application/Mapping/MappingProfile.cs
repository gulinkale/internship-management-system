using AutoMapper;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Models; // Entity’lerin bulunduğu namespace

namespace StajTakipUygulaması.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity <-> DTO
            CreateMap<Belge, BelgeDto>()
                .ForMember(d => d.BelgeTipiAd, m => m.MapFrom(s => s.BelgeTipi != null ? s.BelgeTipi.Ad : null))
                .ReverseMap();

            CreateMap<Belge, BelgeCreateDto>().ReverseMap();
            CreateMap<Belge, BelgeUpdateDto>().ReverseMap();

            CreateMap<StajTakipUygulaması.Models.BelgeTipi, BelgeTipiDto>().ReverseMap();

            CreateMap<BelgeTipi, BelgeTipiDto>().ReverseMap();
        }
    }
}
