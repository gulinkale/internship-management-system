using AutoMapper;
using Application.DTOs;
using StajTakipUygulaması.Models;


namespace StajTakipUygulaması.Application.Mapping
{

    public class StajProfile : Profile
    {
        public StajProfile()
        {
            // LIST
            CreateMap<Staj, StajListDto>()
                .ForMember(d => d.StajyerAdSoyad,
                    m => m.MapFrom(s => s.Stajyer != null ? (s.Stajyer.Ad + " " + s.Stajyer.Soyad) : null))
                .ForMember(d => d.StajTuruAdi,
                    m => m.MapFrom(s => s.StajTuru != null ? s.StajTuru.Ad : null))
                .ForMember(d => d.AktifMi,
                    m => m.MapFrom(s => s.BitisTarihi >= DateTime.Today));

            // DETAIL
            CreateMap<Staj, StajDetailDto>()
                .ForMember(d => d.StajyerAdSoyad,
                    m => m.MapFrom(s => s.Stajyer != null ? (s.Stajyer.Ad + " " + s.Stajyer.Soyad) : null))
                .ForMember(d => d.StajTuruAdi,
                    m => m.MapFrom(s => s.StajTuru != null ? s.StajTuru.Ad : null))
                .ForMember(d => d.AktifMi,
                    m => m.MapFrom(s => s.BitisTarihi >= DateTime.Today));

            // CREATE / UPDATE
            CreateMap<StajCreateDto, Staj>();
            CreateMap<StajUpdateDto, Staj>();
        }
    }
}