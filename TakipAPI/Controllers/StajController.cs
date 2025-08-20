using StajTakipUygulaması.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Models;


namespace StajTakipUygulaması.Controllers
{
    public class StajController : Controller
    {
        private readonly IStajService _stajService;
        private readonly IBelgeTipiService _belgeTipiService;


        public StajController(IStajService stajService, IBelgeTipiService belgeTipiService)
        {
            _stajService = stajService;
            _belgeTipiService = belgeTipiService;
        }

        // Listeleme (isteğe bağlı)
        public async Task<IActionResult> Index()
        {
            var stajlar = await _stajService.GetAllAsync();
            return View(stajlar);
        }

        // Kayıt Ekleme (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staj staj)
        {
            if (ModelState.IsValid)
            {
                await _stajService.AddAsync(staj);
                return RedirectToAction("Index");
            }

            return View(staj);
        }

        // Aktif Stajlar (BitisTarihi bugünden sonra olanlar)
        public async Task<IActionResult> Aktif()
        {
            var stajlar = await _stajService.GetAktifStajlarAsync();
            return View("StajyerListesi/Aktif", stajlar);
        }

        // Tamamlanmış Stajlar (BitisTarihi bugünden önce olanlar)
        public async Task<IActionResult> Tamamlanmıs()
        {
            var stajlar = await _stajService.GetTamamlanmisStajlarAsync();
            return View("StajyerListesi/Tamamlanmıs", stajlar);
        }

        //Stajyer Detay ekranı
        public async Task<IActionResult> Details(int id)
        {
            var staj = await _stajService.GetByIdAsync(id);

            if (staj == null)
                return NotFound();
            
            // Tüm belge tiplerini al (servisten)
            var belgeTipleri = await _belgeTipiService.GetAllAsync();
            ViewBag.BelgeTipleri = belgeTipleri;

            // Explicit olarak doğru yolu belirtiyoruz:
            return View("~/Views/Staj/StajyerListesi/StajyerBilgileri.cshtml", staj);
        }


    }
}
