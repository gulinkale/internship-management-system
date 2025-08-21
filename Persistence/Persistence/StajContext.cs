using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace StajTakipUygulaması.Data
{
    public class StajContext : DbContext
    {
        public StajContext(DbContextOptions<StajContext> options) : base(options) { }

        public DbSet<Stajyer> Stajyerler { get; set; }
        public DbSet<Staj> Stajlar { get; set; }
        public DbSet<Belge> Belgeler { get; set; }
        public DbSet<BelgeTipi> BelgeTipleri { get; set; }
        public DbSet<StajTuru> StajTurleri { get; set; }

        public DbSet<Basvuru> Basvurular { get; set; }
        public DbSet<BasvuruBelge> BasvuruBelgeleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔗 Basvuru ↔️ BasvuruBelge ilişkisi
            modelBuilder.Entity<BasvuruBelge>()
                .HasOne(bb => bb.Basvuru)
                .WithMany(b => b.BasvuruBelgeleri)
                .HasForeignKey(bb => bb.BasvuruID)
                .OnDelete(DeleteBehavior.Cascade);

            // 📌 Belge Tipleri başlangıç verisi
            modelBuilder.Entity<BelgeTipi>().HasData(
                new BelgeTipi { ID = 1, Ad = "Öğrenci Belgesi" },
                new BelgeTipi { ID = 2, Ad = "Transkript" },
                new BelgeTipi { ID = 3, Ad = "Staj Başvuru Formu" },
                new BelgeTipi { ID = 4, Ad = "Bilgi Taahütnamesi" },
                new BelgeTipi { ID = 5, Ad = "Referans Mektubu" },
                new BelgeTipi { ID = 6, Ad = "Staj Onay Raporu" },
                new BelgeTipi { ID = 7, Ad = "Detaylı Staj Durum Raporu" },
                new BelgeTipi { ID = 8, Ad = "Verilen_Referans Mektubu" }
            );

            // 📌 Staj Türleri başlangıç verisi
            modelBuilder.Entity<StajTuru>().HasData(
                new StajTuru { ID = 1, Ad = "Zorunlu" },
                new StajTuru { ID = 2, Ad = "İŞKUR" },
                new StajTuru { ID = 3, Ad = "Kısmi Zamanlı" },
                new StajTuru { ID = 4, Ad = "İşletmede Mesleki Eğitim" }
            );

            base.OnModelCreating(modelBuilder); // her zaman en sonda kalmalı
        }
    }
}
