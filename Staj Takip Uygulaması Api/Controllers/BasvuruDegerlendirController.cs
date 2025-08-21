using Application.DTOs;
using StajTakipUygulaması.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.Interfaces;

namespace StajTakipUygulamas.Ui.Controllers
{
    // UI tarafı: Razor View döner
    [Route("[controller]/[action]")]
    public class BasvuruDegerlendirController : Controller
    {
        private readonly IBasvuruService _service;

        public BasvuruDegerlendirController(IBasvuruService service)
        {
            _service = service;
        }

        // LISTE
        [HttpGet]
        public async Task<IActionResult> Index(string? durum = null)
        {
            var list = await _service.TumBasvurulariGetirAsync(); // List<BasvuruListDto>

            // (İsteğe bağlı) Query ile durum filtresi
            if (!string.IsNullOrWhiteSpace(durum))
                list = list.Where(x => string.Equals(x.Durum, durum, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.SeciliDurum = durum;
            return View(list);
        }

        // DETAY
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Detay(int id)
        {
            var dto = await _service.BasvuruDetayAsync(id); // BasvuruDto?
            if (dto is null) return NotFound();
            return View(dto);
        }

        // OLUSTUR (GET)
        [HttpGet]
        public IActionResult Olustur()
        {
            return View(new BasvuruCreateDto());
        }

        // OLUSTUR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Olustur(BasvuruCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var newId = await _service.BasvuruOlusturAsync(dto);
            TempData["OK"] = "Başvuru oluşturuldu.";
            return RedirectToAction(nameof(Detay), new { id = newId });
        }

        // GUNCELLE (GET)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Guncelle(int id)
        {
            var mevcut = await _service.BasvuruDetayAsync(id);
            if (mevcut is null) return NotFound();

            // Detay DTO'ndan update DTO'su üret (alan adlarını sende nasıl ise ona göre eşle)
            var dto = new BasvuruUpdateDto
            {
                // Id alanı varsa doldur:
                // Id = mevcut.Id,
                // diğer alan map'leri...
            };

            return View(dto);
        }

        // GUNCELLE (POST)
        [HttpPost("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(int id, BasvuruUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            // DTO’da Id varsa route ile eşitle
            var idProp = typeof(BasvuruUpdateDto).GetProperty("Id");
            if (idProp != null) idProp.SetValue(dto, id);

            var ok = await _service.BasvuruGuncelleAsync(dto);
            if (!ok)
            {
                TempData["Hata"] = "Güncelleme başarısız.";
                return View(dto);
            }

            TempData["OK"] = "Başvuru güncellendi.";
            return RedirectToAction(nameof(Detay), new { id });
        }

        // ONAYLA (POST)
        [HttpPost("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var ok = await _service.BasvuruOnaylaAsync(id);
            TempData[ok ? "OK" : "Hata"] = ok ? "Başvuru onaylandı." : "Onay işlemi başarısız.";
            return RedirectToAction(nameof(Detay), new { id });
        }

        // REDDET (POST) – formdan neden alınır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(BasvuruReddetDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Hata"] = "Lütfen reddetme nedenini girin.";
                return RedirectToAction(nameof(Detay), new { id = dto.ID });
            }

            var ok = await _service.BasvuruReddetAsync(dto);
            TempData[ok ? "OK" : "Hata"] = ok ? "Başvuru reddedildi." : "Reddetme işlemi başarısız.";
            return RedirectToAction(nameof(Detay), new { id = dto.ID });
        }

        // REDDEDILENI BEKLEMEYE AL (POST)
        [HttpPost("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BeklemeyeAl(int id)
        {
            var ok = await _service.BeklemeyeAlAsync(id);
            TempData[ok ? "OK" : "Hata"] = ok ? "Başvuru Beklemede durumuna alındı." : "İşlem başarısız.";
            return RedirectToAction(nameof(Detay), new { id });
        }
    }
}
