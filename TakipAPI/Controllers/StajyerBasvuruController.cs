using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Models;
using StajTakipUygulaması.Services.Interfaces;
using StajTakipUygulaması.Data;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace StajTakipUygulaması.Controllers
{
    public class StajyerBasvuruController : Controller
    {
        private readonly IStajyerService _stajyerService;
        private readonly StajContext _context;
        private readonly IEmailSender _email;


        public StajyerBasvuruController(IStajyerService stajyerService, StajContext context, IEmailSender email)
        {
            _stajyerService = stajyerService;
            _context = context;
            _email = email; 
        }

        // 🔍 Listeleme
        public async Task<IActionResult> Index()
        {
            var basvurular = await _context.Basvurular
                .Include(b => b.StajTuru)
                .Include(b => b.BasvuruBelgeleri)
                .ToListAsync();

            return View(basvurular);
        }

        // 📄 Detay
        public async Task<IActionResult> Details(int id)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.BasvuruBelgeleri)
                .FirstOrDefaultAsync(b => b.ID == id);

            if (basvuru == null) return NotFound();

            return View(basvuru);
        }

        // 📋 Form (GET)
        public IActionResult Create()
        {
            ViewBag.StajTurleri = _context.StajTurleri
                .Select(st => new SelectListItem
                {
                    Value = st.ID.ToString(),
                    Text = st.Ad
                }).ToList();

            if (TempData["Mesaj"] != null)
                ViewBag.Mesaj = TempData["Mesaj"];

            return View(new OgrenciViewModel());
        }

        // 📥 Form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OgrenciViewModel model)
        {
            
            
            // 🔒 TC tekrar başvuru kontrolü
            var tc = model?.Stajyer?.TCKimlikNo?.Trim();

if (string.IsNullOrWhiteSpace(tc))
{
    ModelState.AddModelError("Stajyer.TCKimlikNo", "TC Kimlik No zorunludur.");
}
else
{
    // 1) Sistemde aynı TC ile herhangi bir başvuru var mı?
    var varMiBasvuru = await _context.Basvurular
        .AnyAsync(x => x.TCKimlikNo == tc);

    // 2) Zaten stajyer olarak kayıtlı mı?
                var varMiStajyer = await _context.Stajyerler
        .AnyAsync(x => x.TCKimlikNo == tc);

    if (varMiBasvuru || varMiStajyer)
    {
        ModelState.AddModelError("Stajyer.TCKimlikNo",
            "Bu TC ile sistemde zaten bir başvuru veya staj kaydı var. Yeni başvuru alınamaz.");
    }
}


            if (!ModelState.IsValid)
            {
                ViewBag.StajTurleri = _context.StajTurleri
                    .Select(st => new SelectListItem
                    {
                        Value = st.ID.ToString(),
                        Text = st.Ad
                    }).ToList();
                return View(model);
            }

            // ✅ Başvuru nesnesi oluştur
            var basvuru = new Basvuru
            {
                Ad = model.Stajyer.Ad,
                Soyad = model.Stajyer.Soyad,
                TCKimlikNo = model.Stajyer.TCKimlikNo,
                Cinsiyet = model.Stajyer.Cinsiyet,
                TelNo = model.Stajyer.TelNo,
                Email = model.Stajyer.Email,
                Adres = model.Stajyer.Adres,
                Universite = model.Stajyer.Universite,
                Fakulte = model.Stajyer.Fakulte,
                Bolum = model.Stajyer.Bolum,
                Sinif = model.Stajyer.Sinif,
                BaslamaYili = model.Stajyer.BaslamaYili,
                OgrenciNo = model.Stajyer.OgrenciNo,
                Departman = model.Staj.Departman,
                SorumluID = model.Staj.SorumluID,
                Yetkiler = model.Staj.Yetkiler,
                BaslamaTarihi = model.Staj.BaslamaTarihi,
                BitisTarihi = model.Staj.BitisTarihi,
                StajTuruID = model.Staj.StajTuruID,
                BasvuruTarihi = DateTime.Now
            };

            // ✅ Belgeleri işle
            var belgeler = new List<IFormFile?> {
                model.OgrenciBelgesi,
                model.Transkript,
                model.BasvuruFormu,
                model.Taahutname,
                model.Referans
            };

            string[] belgeAdlari = {
                "Öğrenci Belgesi",
                "Transkript",
                "Başvuru Formu",
                "Taahhütname",
                "Referans Mektubu"
            };

            for (int i = 0; i < belgeler.Count; i++)
            {
                var file = belgeler[i];
                if (file != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "belgeler");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueName = Guid.NewGuid() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    basvuru.BasvuruBelgeleri ??= new List<BasvuruBelge>();
                    basvuru.BasvuruBelgeleri.Add(new BasvuruBelge
                    {
                        BelgeAdı = belgeAdlari[i],
                        Yolu = "/belgeler/" + uniqueName,
                        Açıklama = "",
                        BelgeTipiID = i + 1
                    });
                }
            }

            // >>> FOTOĞRAF BELGESİ (formdan geldiyse) <<<
if (model.Fotograf != null && model.Fotograf.Length > 0)
{
    // "Fotoğraf" belge tipinin ID'sini bul
    var fotoTipId = await _context.BelgeTipleri
        .Where(t => t.Ad == "Fotoğraf")
        .Select(t => t.ID)
        .FirstOrDefaultAsync();

    if (fotoTipId > 0) // tip bulunduysa
    {
        // Dosyayı kaydet
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "belgeler");
        Directory.CreateDirectory(uploadsFolder);

        var ext = Path.GetExtension(model.Fotograf.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
            await model.Fotograf.CopyToAsync(stream);

        // BasvuruBelge kaydını ekle
        basvuru.BasvuruBelgeleri ??= new List<BasvuruBelge>();
        basvuru.BasvuruBelgeleri.Add(new BasvuruBelge
        {
            BelgeTipiID = fotoTipId,
            BelgeAdı    = "Fotoğraf",
            Yolu        = "/belgeler/" + fileName, // public web yolu
            Açıklama    = ""
        });
    }
}


            // ✅ Veritabanına ekle
            _context.Basvurular.Add(basvuru);
            await _context.SaveChangesAsync();

            // 📧 Başvuru alındı maili
if (!string.IsNullOrWhiteSpace(basvuru.Email))
{
    var subject = "Staj Başvurunuz Alındı";
    var body = $@"
        <p>Merhaba <b>{basvuru.Ad} {basvuru.Soyad}</b>,</p>
        <p>Staj başvurunuz sistemimize <b>başarıyla ulaştı</b>.</p>
        <p>
            <b>Başvuru No:</b> {basvuru.ID}<br/>
            <b>Tarih:</b> {basvuru.BasvuruTarihi:dd.MM.yyyy HH:mm}<br/>
            <b>Durum:</b> Beklemede
        </p>
        <p>Değerlendirme tamamlandığında size e-posta ile bilgi verilecektir.</p>
        <p>İyi günler dileriz.</p>";
    try { await _email.SendAsync(basvuru.Email, subject, body); } catch { /* mail hatası uygulamayı bozmasın */ }
}

            TempData["Mesaj"] = "✅ Başvuru başarıyla kaydedildi.";
            return RedirectToAction("Create");
        }
    }
}
