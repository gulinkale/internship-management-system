using AutoMapper;
using StajTakipUygulamasi.Application.DTOs.BasvuruBelge;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Domain.Entities;

namespace StajTakipUygulaması.Application.Mapping
{
    public class BasvuruBelgeProfile : Profile
    {
        public BasvuruBelgeProfile()
        {
            // LIST
            CreateMap<BasvuruBelge, BasvuruBelgeListDto>()
                .ForMember(d => d.BelgeAdi, m => m.MapFrom(s => s.BelgeAdı))
                .ForMember(d => d.DosyaYolu, m => m.MapFrom(s => s.Yolu))
                .ForMember(d => d.BelgeTipiAdi, m => m.MapFrom(s => s.BelgeTipi != null ? s.BelgeTipi.Ad : null));

            // DETAIL
            CreateMap<BasvuruBelge, BasvuruBelgeDetailDto>()
                .ForMember(d => d.BelgeAdi, m => m.MapFrom(s => s.BelgeAdı))
                .ForMember(d => d.Aciklama, m => m.MapFrom(s => s.Açıklama))
                .ForMember(d => d.DosyaYolu, m => m.MapFrom(s => s.Yolu))
                .ForMember(d => d.BelgeTipiAdi, m => m.MapFrom(s => s.BelgeTipi != null ? s.BelgeTipi.Ad : null))
                .ForMember(d => d.BasvuruAdSoyad, m => m.MapFrom(s => s.Basvuru != null ? (s.Basvuru.Ad + " " + s.Basvuru.Soyad) : null));

            // CREATE / UPDATE
            CreateMap<BasvuruBelgeCreateDto, BasvuruBelge>()
                .ForMember(d => d.BelgeAdı, m => m.MapFrom(s => s.BelgeAdi))
                .ForMember(d => d.Açıklama, m => m.MapFrom(s => s.Aciklama))
                .ForMember(d => d.Yolu, m => m.MapFrom(s => s.DosyaYolu));

            CreateMap<BasvuruBelgeUpdateDto, BasvuruBelge>()
                .ForMember(d => d.BelgeAdı, m => m.MapFrom(s => s.BelgeAdi))
                .ForMember(d => d.Açıklama, m => m.MapFrom(s => s.Aciklama))
                .ForMember(d => d.Yolu, m => m.MapFrom(s => s.DosyaYolu));
        }
    }
}