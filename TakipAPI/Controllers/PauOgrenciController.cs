using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Models;
using StajTakipUygulaması.Services.Interfaces;
using StajTakipUygulaması.Data;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StajTakipUygulaması.Controllers
{
    public class PauOgrenciController : Controller
    {
        private readonly IStajyerService _stajyerService;
        private readonly StajContext _context;

        public PauOgrenciController(IStajyerService stajyerService, StajContext context)
        {
            _stajyerService = stajyerService;
            _context = context;
        }

        // 🔍 Listeleme
        public async Task<IActionResult> Index()
        {
            var stajyerler = await _stajyerService.GetAllAsync();
            return View(stajyerler);
        }

        // 📄 Detay
        public async Task<IActionResult> Details(int id)
        {
            var stajyer = await _stajyerService.GetByIdAsync(id);
            if (stajyer == null) return NotFound();
            return View(stajyer);
        }

        // 👨‍🎓 Kayıt Formu (GET)
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

        // 📩 Kayıt Formu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OgrenciViewModel model)
        {
            Console.WriteLine("Create POST tetiklendi");

            model.Stajyer ??= new Stajyer();
            model.Staj ??= new Staj();

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState geçersiz!");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"🔴 Hata [{entry.Key}]: {error.ErrorMessage}");
                    }
                }

                ViewBag.StajTurleri = _context.StajTurleri
                    .Select(st => new SelectListItem
                    {
                        Value = st.ID.ToString(),
                        Text = st.Ad
                    }).ToList();

                return View(model);
            }

            // ✅ 1. Stajyer Kaydı
            _context.Stajyerler.Add(model.Stajyer);
            await _stajyerService.AddAsync(model.Stajyer);

            // ✅ 2. Staj Kaydı
            model.Staj.StajyerID = model.Stajyer.ID;
            _context.Stajlar.Add(model.Staj);
            await _context.SaveChangesAsync();

            // ✅ 3. Belgeler
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

            // ✅ Kullanıcıya mesaj göstermek için TempData kullanıyoruz
            TempData["Mesaj"] = "✅ Öğrenci başarıyla kaydedildi.";

            // ✅ Aynı form sayfasına boş modelle geri dönüyoruz
            return RedirectToAction("Create");
        }
    }
}
