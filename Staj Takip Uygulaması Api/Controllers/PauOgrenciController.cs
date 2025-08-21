using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Domain.Entities;
using StajTakipUygulaması.Models;

namespace StajTakipUygulamas.Ui.Controllers
{
    [Route("[controller]/[action]")]
    public class PauOgrenciController : Controller
    {
        private readonly IStajyerService _stajyerService;
        private readonly StajContext _context;

        public PauOgrenciController(IStajyerService stajyerService, StajContext context)
        {
            _stajyerService = stajyerService;
            _context = context;
        }

        // LISTE
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stajyerler = await _stajyerService.GetAllAsync();
            return View(stajyerler); // Views/PauOgrenci/Index.cshtml -> @model IReadOnlyList<StajyerDto>
        }

        // DETAY
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var stajyer = await _stajyerService.GetByIdAsync(id);
            return stajyer is null ? NotFound() : View(stajyer); // Views/PauOgrenci/Details.cshtml -> @model StajyerDto
        }

        // OLUSTUR (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StajTurleri = _context.StajTurleri
                .Select(st => new SelectListItem { Value = st.ID.ToString(), Text = st.Ad })
                .ToList();

            if (TempData["Mesaj"] != null) ViewBag.Mesaj = TempData["Mesaj"];
            return View(new OgrenciViewModel()); // Views/PauOgrenci/Create.cshtml -> @model OgrenciViewModel
        }

        // OLUSTUR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OgrenciViewModel model)
        {
            model.Stajyer ??= new Stajyer();
            model.Staj ??= new Staj();

            if (!ModelState.IsValid)
            {
                ViewBag.StajTurleri = _context.StajTurleri
                    .Select(st => new SelectListItem { Value = st.ID.ToString(), Text = st.Ad })
                    .ToList();
                return View(model);
            }

            // 1) Stajyer
            _context.Stajyerler.Add(model.Stajyer);
            await _stajyerService.AddAsync(new StajTakipUygulaması.Application.DTOs.StajyerCreateDto
            {
                // Eğer DTO’nun alanları entity’den farklıysa burada map et
                Universite = model.Stajyer.Universite,
                OgrenciNo = model.Stajyer.OgrenciNo,
                Bolum = model.Stajyer.Bolum,
                Fakulte = model.Stajyer.Fakulte,
                BaslamaYili = model.Stajyer.BaslamaYili,
                Sinif = model.Stajyer.Sinif,
                PAU_ogrencisi_mi = model.Stajyer.PAU_ogrencisi_mi,
                Ad = model.Stajyer.Ad,
                Soyad = model.Stajyer.Soyad,
                TCKimlikNo = model.Stajyer.TCKimlikNo,
                DogumTarihi = model.Stajyer.DogumTarihi,
                Cinsiyet = model.Stajyer.Cinsiyet,
                TelNo = model.Stajyer.TelNo,
                Email = model.Stajyer.Email,
                Adres = model.Stajyer.Adres
            });

            // 2) Staj
            model.Staj.StajyerID = model.Stajyer.ID;
            _context.Stajlar.Add(model.Staj);
            await _context.SaveChangesAsync();

            // 3) Belgeler (varsa)
            var belgeler = new List<IFormFile?> { model.OgrenciBelgesi, model.Transkript, model.BasvuruFormu, model.Taahutname, model.Referans };
            string[] belgeAdlari = { "Öğrenci Belgesi", "Transkript", "Başvuru Formu", "Taahhütname", "Referans Mektubu" };

            for (int i = 0; i < belgeler.Count; i++)
            {
                var file = belgeler[i];
                if (file == null) continue;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "belgeler");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                _context.Belgeler.Add(new Belge
                {
                    BelgeAdı = belgeAdlari[i],
                    Yolu = "/belgeler/" + uniqueName,
                    Açıklama = "",
                    StajID = model.Staj.ID,
                    BelgeTipiID = i + 1
                });
            }

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "✅ Öğrenci başarıyla kaydedildi.";
            return RedirectToAction(nameof(Create));
        }
    }
}
