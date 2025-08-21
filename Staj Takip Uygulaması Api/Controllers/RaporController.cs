using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Data;

namespace StajTakipUygulaması.Controllers
{
    public class RaporController : Controller
    {
        private readonly StajContext _context;

        public RaporController(StajContext context)
        {
            _context = context;
        }

        // Arama ekranı (GET)
        [HttpGet]
        public IActionResult Ara(int belgeTipId)
        {
            // Tek isim kullan: BelgeTipId
            ViewBag.BelgeTipId = belgeTipId;
            return View();
        }

        // Arama sonucu (POST)
        [HttpPost]
        public async Task<IActionResult> Ara(string arama, int belgeTipId)
        {
            var stajyerler = await _context.Stajyerler
                .Include(s => s.Stajlar)
                    .ThenInclude(staj => staj.Belgeler)
                .Where(s =>
                    s.TCKimlikNo == arama ||
                    (s.Ad + " " + s.Soyad).Contains(arama) ||
                    s.OgrenciNo == arama
                )
                .ToListAsync();

            // Tek isim kullan: BelgeTipId
            ViewBag.BelgeTipId = belgeTipId;

            return View("Sonuc", stajyerler);
        }
    }
}
