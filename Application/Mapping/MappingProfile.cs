using AutoMapper;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Domain.Entities;

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

            CreateMap<StajTakipUygulaması.Domain.Entities.BelgeTipi, BelgeTipiDto>().ReverseMap();

            CreateMap<BelgeTipi, BelgeTipiDto>().ReverseMap();
        }
    }
}
