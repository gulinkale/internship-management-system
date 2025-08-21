using AutoMapper;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Domain.Entities;

namespace StajTakipUygulaması.Application.Mapping
{
    public class BasvuruProfile : Profile
    {
        public BasvuruProfile()
        {
            // LISTE
            CreateMap<Basvuru, BasvuruListDto>()
                .ForMember(d => d.AdSoyad, m => m.MapFrom(s => s.Ad + " " + s.Soyad))
                .ForMember(d => d.Durum, m => m.MapFrom(s => s.Durum.ToString()))
                .ForMember(d => d.StajTuruAdi, m => m.MapFrom(s => s.StajTuru != null ? s.StajTuru.Ad : null))
                .ForMember(d => d.BelgeSayisi, m => m.MapFrom(s => s.BasvuruBelgeleri != null ? s.BasvuruBelgeleri.Count : 0));

            // CREATE/UPDATE
            CreateMap<BasvuruCreateDto, Basvuru>();
            CreateMap<BasvuruUpdateDto, Basvuru>();
        }
    }
}