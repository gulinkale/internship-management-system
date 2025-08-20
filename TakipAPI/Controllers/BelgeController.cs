using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Models;
using System.IO;

namespace StajTakipUygulaması.Controllers
{
    public class BelgeController : Controller
    {
        private readonly IBelgeService _belgeService;
        private readonly IStajService _stajService;
        private readonly IBelgeTipiService _belgeTipiService;
        private readonly StajContext _context;

        public BelgeController(
            IBelgeService belgeService,
            IStajService stajService,
            IBelgeTipiService belgeTipiService,
            StajContext context)
        {
            _belgeService = belgeService;
            _stajService = stajService;
            _belgeTipiService = belgeTipiService;
            _context = context;
        }

        // ✅ Belge Listeleme
        public async Task<IActionResult> Index()
        {
            var belgeler = await _belgeService.GetAllAsync();
            return View(belgeler);
        }

        // ✅ Belge Yükleme (GET) — tek kaynak burası
        [HttpGet]
        public IActionResult Yukle(int stajId, int belgeTipId, string? returnUrl)
        {
            var model = new Belge { StajID = stajId, BelgeTipiID = belgeTipId };

            ViewBag.BelgeTipAd = _context.BelgeTipleri
                                         .Where(x => x.ID == belgeTipId)
                                         .Select(x => x.Ad)
                                         .FirstOrDefault();

            // Yükleme sonrası döneceğimiz yer
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? Url.Action("Details", "Staj", new { id = stajId })
                : returnUrl;

            return View(model); // Views/Belge/Yukle.cshtml
        }

        // ✅ Belge Yükleme (POST) — tek kaynak burası
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Yukle(IFormFile dosya, int StajID, int BelgeTipiID, string? Aciklama, string? returnUrl)
        {
            var belgeTipi = await _context.BelgeTipleri.FindAsync(BelgeTipiID);
            if (belgeTipi == null)
            {
                TempData["Hata"] = "Geçersiz belge tipi seçildi.";
                return RedirectToAction(nameof(Yukle), new { stajId = StajID, belgeTipId = BelgeTipiID, returnUrl });
            }

            if (dosya == null || dosya.Length == 0)
            {
                TempData["Hata"] = "Dosya seçilmedi.";
                return RedirectToAction(nameof(Yukle), new { stajId = StajID, belgeTipId = BelgeTipiID, returnUrl });
            }

            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "belgeler");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueName = Guid.NewGuid() + "_" + dosya.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dosya.CopyToAsync(stream);
                }

                var yeniBelge = new Belge
                {
                    BelgeTipiID = BelgeTipiID,
                    StajID      = StajID,
                    Yolu        = "/belgeler/" + uniqueName,        // web yolu
                    BelgeAdı    = belgeTipi.Ad,                    // tip adından
                    Açıklama    = string.IsNullOrWhiteSpace(Aciklama) ? "Yüklendi" : Aciklama.Trim()
                };

                _context.Belgeler.Add(yeniBelge);
                await _context.SaveChangesAsync();

                TempData["OK"] = "Belge başarıyla yüklendi.";

                // Geldiğimiz yere dön (ör: Rapor/Sonuc) yoksa Staj/Details
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Details", "Staj", new { id = StajID });
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Yukle), new { stajId = StajID, belgeTipId = BelgeTipiID, returnUrl });
            }
        }

        // ✅ GÜNCELLE (GET)
        [HttpGet]
        public async Task<IActionResult> Guncelle(int id, string? returnUrl)
        {
            var belge = await _belgeService.GetByIdAsync(id);
            if (belge == null)
                return NotFound();

            ViewBag.BelgeTipAd = _context.BelgeTipleri.FirstOrDefault(bt => bt.ID == belge.BelgeTipiID)?.Ad;
            ViewBag.ControllerName = "Belge";
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? Url.Action("Details", "Staj", new { id = belge.StajID })
                : returnUrl;

            return View(belge); // Views/Belge/Guncelle.cshtml
        }

        // ✅ GÜNCELLE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(int id, IFormFile yeniDosya, string? returnUrl)
        {
            var belge = await _belgeService.GetByIdAsync(id);
            if (belge == null)
                return NotFound();

            if (yeniDosya != null && yeniDosya.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "belgeler");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueName = Guid.NewGuid() + "_" + yeniDosya.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await yeniDosya.CopyToAsync(stream);
                }

                belge.Yolu = "/belgeler/" + uniqueName;
                belge.Açıklama = "Güncellendi";
                await _belgeService.UpdateAsync(belge);
            }

            // Geldiğimiz yere dön, yoksa Staj/Details
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Details", "Staj", new { id = belge.StajID });
        }
    }
}
