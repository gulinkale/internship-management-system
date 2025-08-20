using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Models;
using System.IO;

namespace StajTakipUygulaması.Controllers
{
    public class PauDisiOgrenciController : Controller
    {
        private readonly IStajyerService _stajyerService;
        private readonly StajContext _context;

        public PauDisiOgrenciController(IStajyerService stajyerService, StajContext context)
        {
            _stajyerService = stajyerService;
            _context = context;
        }

        // 🔍 Detay
        public async Task<IActionResult> Details(int id)
        {
            var stajyer = await _stajyerService.GetByIdAsync(id);
            if (stajyer == null) return NotFound();
            return View(stajyer);
        }

        // 📄 Form (GET)
         public IActionResult Create()
        {
            ViewBag.StajTurleri = _context.StajTurleri
                .Select(st => new SelectListItem
                {
                    Value = st.ID.ToString(),
                    Text = st.Ad
                }).ToList();

            // Eğer TempData varsa ViewBag’e aktar
            if (TempData["Mesaj"] != null)
                ViewBag.Mesaj = TempData["Mesaj"];

            return View(new OgrenciViewModel());
        }

        // 📝 Form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OgrenciViewModel model)
        {
            // Güvenlik için null kontrolü
            model.Stajyer ??= new Stajyer();
            model.Staj ??= new Staj();

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

            // 1. Stajyer kayıt
            _context.Stajyerler.Add(model.Stajyer);
            await _stajyerService.AddAsync(model.Stajyer);

            // 2. Staj kayıt
            model.Staj.StajyerID = model.Stajyer.ID;
            _context.Stajlar.Add(model.Staj);
            await _context.SaveChangesAsync();

            // 3. Belgeler
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

                    _context.Belgeler.Add(new Belge
                    {
                        BelgeAdı = belgeAdlari[i],
                        Yolu = "/belgeler/" + uniqueName,
                        Açıklama = "",
                        StajID = model.Staj.ID,
                        BelgeTipiID = i + 1
                    });
                }
            }

            await _context.SaveChangesAsync();

            // 🎉 Başarı mesajı göstermek için ViewBag
            ViewBag.Mesaj = "PAÜ Dışı Öğrenci başarıyla kaydedildi.";
            ModelState.Clear(); // form temizliği
            return View(new OgrenciViewModel());
        }
    }
}
