using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Models;
using System.Reflection.Emit;


namespace StajTakipUygulaması.Controllers
{
    public class BasvuruDegerlendirController : Controller
    {
        private readonly StajContext _context;
        private readonly IEmailSender _email;
        public BasvuruDegerlendirController(StajContext context, IEmailSender email)
        {
            _context = context;
            _email = email;
        }
        // LİSTE
        [HttpGet]
        public async Task<IActionResult> Index(BasvuruDurumu? durum)
        {
            // Parametre yoksa BEKLEMEDE listele
            if (!durum.HasValue)
                durum = BasvuruDurumu.Beklemede;

            var list = await _context.Basvurular
                                     .AsNoTracking()
                                     .Include(b => b.StajTuru)
                                     .Where(b => b.Durum == durum.Value)   // seçilen duruma göre her zaman filtrele
                                     .OrderByDescending(b => b.ID)
                                     .ToListAsync();

            ViewBag.SeciliDurum = durum.Value.ToString();
            return View(list);
        }






        // DETAY – Başvuru + Belgeler
        [HttpGet]
        public async Task<IActionResult> Detay(int id)
        {
            var b = await _context.Basvurular
                                  .Include(x => x.StajTuru)
                                  .Include(x => x.BasvuruBelgeleri)
                                      .ThenInclude(bb => bb.BelgeTipi)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(x => x.ID == id);

            if (b == null) return NotFound();

            ViewBag.BelgeTipleri = await _context.BelgeTipleri
                                                 .AsNoTracking()
                                                 .OrderBy(t => t.ID)
                                                 .Take(5)
                                                 .ToListAsync();

            return View(b); // Views/BasvuruDegerlendir/Detay.cshtml -> @model Basvuru
        }

        // KABUL ET (GET)
        [HttpGet]
        public async Task<IActionResult> KabulEt(int id)
        {
            var b = await _context.Basvurular
                                  .Include(x => x.StajTuru)
                                  .FirstOrDefaultAsync(x => x.ID == id);
            if (b == null) return NotFound();
            return View(b); // Views/BasvuruDegerlendir/KabulEt.cshtml -> @model Basvuru
        }

        // KABUL ET (POST) – Onay alanları + Stajyer/Staj/Belge aktarımı
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KabulEt(
            int id,
            [Bind("Departman,SorumluID,Yetkiler,BaslamaTarihi,BitisTarihi")] Basvuru input)
        {
            // 1) Başvuru + belgeler
            var b = await _context.Basvurular
                                  .Include(x => x.BasvuruBelgeleri)
                                      .ThenInclude(bb => bb.BelgeTipi)
                                  .FirstOrDefaultAsync(x => x.ID == id);
            if (b == null) return NotFound();

            // 2) Formdan gelen alanları normalize et
            b.Departman = string.IsNullOrWhiteSpace(input.Departman) ? "Belirlenecek" : input.Departman!.Trim();
            b.SorumluID = string.IsNullOrWhiteSpace(input.SorumluID) ? "Belirlenecek" : input.SorumluID!.Trim();
            b.Yetkiler = input.Yetkiler ?? "";
            b.BaslamaTarihi = input.BaslamaTarihi ?? DateTime.Today;
            b.BitisTarihi = input.BitisTarihi ?? DateTime.Today.AddMonths(1);

            // 3) FK + tarihler kontrol
            var stajTuruValid = b.StajTuruID > 0 &&
                                await _context.StajTurleri.AnyAsync(t => t.ID == b.StajTuruID);
            if (!stajTuruValid || b.BaslamaTarihi == null || b.BitisTarihi == null)
            {
                TempData["Hata"] = "Onay hatası: Geçerli bir staj türü ve tarihleri seçin.";
                return RedirectToAction(nameof(Index));
            }

            // 4) Null olmayan tarihler
            var baslama = b.BaslamaTarihi ?? DateTime.Today;
            var bitis = b.BitisTarihi ?? DateTime.Today.AddMonths(1);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 5) Stajyer
                var stajyer = new Stajyer
                {
                    Ad = b.Ad ?? "Belirtilmedi",
                    Soyad = b.Soyad ?? "Belirtilmedi",
                    TCKimlikNo = b.TCKimlikNo ?? "00000000000",
                    OgrenciNo = b.OgrenciNo,
                    DogumTarihi = b.DogumTarihi == default ? DateTime.Today.AddYears(-20) : b.DogumTarihi,
                    Cinsiyet = b.Cinsiyet ?? "Belirtilmedi",
                    TelNo = b.TelNo ?? "0",
                    Email = b.Email ?? "yok@example.com",
                    Adres = b.Adres ?? "Belirtilmedi",
                    Universite = b.Universite ?? "Belirtilmedi",
                    Fakulte = b.Fakulte ?? "Belirtilmedi",
                    Bolum = b.Bolum ?? "Belirtilmedi",
                    BaslamaYili = b.BaslamaYili,
                    Sinif = b.Sinif,
                    PAU_ogrencisi_mi = false
                };
                _context.Stajyerler.Add(stajyer);
                await _context.SaveChangesAsync(); // stajyer.ID

                // 6) Staj
                var staj = new Staj
                {
                    StajyerID = stajyer.ID,
                    Departman = string.IsNullOrWhiteSpace(b.Departman) ? "Belirlenecek" : b.Departman!,
                    SorumluID = string.IsNullOrWhiteSpace(b.SorumluID) ? "Belirlenecek" : b.SorumluID!,
                    Yetkiler = b.Yetkiler ?? "",
                    BaslamaTarihi = baslama,
                    BitisTarihi = bitis,
                    StajTuruID = b.StajTuruID
                };
                _context.Stajlar.Add(staj);
                await _context.SaveChangesAsync(); // staj.ID

                // 7) Belgeler
                if (b.BasvuruBelgeleri != null)
                {
                    foreach (var bb in b.BasvuruBelgeleri)
                    {
                        if (string.IsNullOrWhiteSpace(bb.Yolu)) continue;

                        var belgeAdi = !string.IsNullOrWhiteSpace(bb.BelgeAdı)
                                        ? bb.BelgeAdı
                                        : (bb.BelgeTipi?.Ad ?? "Belge");

                        _context.Belgeler.Add(new Belge
                        {
                            StajID = staj.ID,
                            BelgeTipiID = bb.BelgeTipiID,
                            Yolu = bb.Yolu,
                            BelgeAdı = belgeAdi,
                            Açıklama = bb.Açıklama ?? ""
                        });
                    }
                }

                // 8) Başvuruyu güncelle/sil
                b.Durum = BasvuruDurumu.Onaylandi;       // onaylandı olarak işaretle
                // _context.Basvurular.Remove(b);        // tamamen kaldırmak istersen bu satırı aç

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // ✅ Onay maili
                if (!string.IsNullOrWhiteSpace(b.Email))
                {
                    var subject = "Staj Başvurunuz Onaylandı";
                    var body =
                $@"Merhaba {b.Ad} {b.Soyad},

Staj başvurunuz onaylanmıştır.

Staj Türü : {(await _context.StajTurleri.Where(t => t.ID == b.StajTuruID).Select(t => t.Ad).FirstOrDefaultAsync())}
Departman : {b.Departman}
Sorumlu   : {b.SorumluID}
Tarih     : {(b.BaslamaTarihi ?? DateTime.Today):dd.MM.yyyy} - {(b.BitisTarihi ?? DateTime.Today.AddMonths(1)):dd.MM.yyyy}

İyi çalışmalar.";
                    try { await _email.SendAsync(b.Email, subject, body); } catch { /* loglayabilirsiniz */ }
                }


                TempData["OK"] = "Başvuru onaylandı; Stajyer, Staj ve Belgeler aktarıldı.";
                // Bekleyenleri gösteren listeye dön (onaylanan kayıt görünmez)
                return RedirectToAction(nameof(Index), new { durum = BasvuruDurumu.Beklemede });
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                TempData["Hata"] = "DB Hatası: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                TempData["Hata"] = "Onay sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // REDDET
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id, string? neden)
        {
            var b = await _context.Basvurular.FindAsync(id);
            if (b == null) return NotFound();

            b.Durum = BasvuruDurumu.Reddedildi;
            b.RedNedeni = string.IsNullOrWhiteSpace(neden) ? null : neden.Trim();
            b.RedTarihi = DateTime.Now;

            await _context.SaveChangesAsync();

            // ✅ Red maili

            //kalıcı red için drop from db


            if (!string.IsNullOrWhiteSpace(b.Email))
            {
                var subject = "Staj Başvurunuz Hakkında";
                var body =
            $@"Merhaba {b.Ad} {b.Soyad},

Üzgünüz, staj başvurunuz reddedilmiştir.
{(string.IsNullOrWhiteSpace(b.RedNedeni) ? "" : "Neden: " + b.RedNedeni)}

Yeni bir başvuruda bulunmadan önce koşulları kontrol edebilirsiniz.";
                try { await _email.SendAsync(b.Email, subject, body); } catch { /* loglayabilirsiniz */ }
            }



            TempData["OK"] = "Başvuru reddedildi.";
            return RedirectToAction(nameof(Index));

        }

        // REDDEDİLENİ BEKLEMEYE AL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BeklemeyeAl(int id)
        {
            var b = await _context.Basvurular.FindAsync(id);
            if (b == null) return NotFound();

            // yalnızca reddedilmişleri beklemeye al
            if (b.Durum != BasvuruDurumu.Reddedildi)
            {
                TempData["Hata"] = "Bu kayıt reddedilmiş durumda değil.";
                return RedirectToAction(nameof(Index), new { durum = b.Durum });
            }

            b.Durum = BasvuruDurumu.Beklemede;
            b.RedNedeni = null;
            b.RedTarihi = null;
            await _context.SaveChangesAsync();

            // (opsiyonel) bilgilendirme e-postası
            if (!string.IsNullOrWhiteSpace(b.Email))
            {
                try
                {
                    await _email.SendAsync(
                        b.Email,
                        "Başvurunuz tekrar değerlendirmeye alındı",
                        $"Merhaba {b.Ad} {b.Soyad}, başvurunuz tekrar değerlendirmeye alınmıştır."
                    );
                }
                catch { /* loglanabilir */ }
            }

            TempData["OK"] = "Başvuru Beklemede durumuna alındı.";
            // İSTEĞİN GEREĞİ => Bekleyenlerin listesine dön
            return RedirectToAction(nameof(Index), new { durum = BasvuruDurumu.Beklemede });
        }


    }
}
