using AutoMapper;
using Application.DTOs;
using StajTakipUygulaması.Models;

public class StajTuruProfile : Profile
{
    public StajTuruProfile()
    {
        CreateMap<StajTuru, StajTuruListDto>()
            .ForMember(d => d.StajSayisi, m => m.MapFrom(s => s.Stajlar != null ? s.Stajlar.Count : 0));

        CreateMap<StajTuru, StajTuruDetailDto>()
            .ForMember(d => d.StajSayisi, m => m.MapFrom(s => s.Stajlar != null ? s.Stajlar.Count : 0));

        CreateMap<StajTuruCreateDto, StajTuru>();
        CreateMap<StajTuruUpdateDto, StajTuru>();
    }
}
