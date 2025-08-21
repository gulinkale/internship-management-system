// Proje: StajTakipUygulamasi.Web
// Dosya: Controllers/BelgeController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StajTakipUygulamasi.Web.Controllers
{
    public class BelgeController : Controller
    {
        private readonly HttpClient _http;
        public BelgeController(IHttpClientFactory f) => _http = f.CreateClient("Api");

        // İlgili stajın belgeleri (opsiyonel)
        [HttpGet]
        public async Task<IActionResult> Index(int stajId)
        {
            var resp = await _http.GetAsync($"api/belge?stajId={stajId}");
            if (!resp.IsSuccessStatusCode)
            {
                TempData["Hata"] = "Belge listesi alınamadı.";
                return View(new List<object>());
            }
            var data = await resp.Content.ReadFromJsonAsync<List<dynamic>>();
            ViewBag.StajId = stajId;
            return View(data!);
        }

        // Yükle (GET) — sadece form
        [HttpGet]
        public IActionResult Yukle(int stajId, int belgeTipId, string? returnUrl)
        {
            ViewBag.StajId = stajId;
            ViewBag.BelgeTipId = belgeTipId;
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? Url.Action("Details", "Staj", new { id = stajId })
                : returnUrl;
            return View();
        }

        // Yükle (POST) — API'ye multipart gönder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Yukle(IFormFile dosya, int StajID, int BelgeTipiID, string? Aciklama, string? returnUrl)
        {
            if (dosya == null || dosya.Length == 0)
            {
                TempData["Hata"] = "Dosya seçilmedi.";
                return RedirectToAction(nameof(Yukle), new { stajId = StajID, belgeTipId = BelgeTipiID, returnUrl });
            }

            using var content = new MultipartFormDataContent();
            var stream = dosya.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(dosya.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "dosya", dosya.FileName);
            content.Add(new StringContent(StajID.ToString()), "stajId");
            content.Add(new StringContent(BelgeTipiID.ToString()), "belgeTipId");
            content.Add(new StringContent(Aciklama ?? ""), "aciklama");

            var resp = await _http.PostAsync("api/belge/upload", content);
            if (!resp.IsSuccessStatusCode)
            {
                TempData["Hata"] = "Yükleme başarısız.";
                return RedirectToAction(nameof(Yukle), new { stajId = StajID, belgeTipId = BelgeTipiID, returnUrl });
            }

            TempData["OK"] = "Belge başarıyla yüklendi.";
            if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Details", "Staj", new { id = StajID });
        }

        // Güncelle (GET) — mevcut belgeyi getirip form göster
        [HttpGet]
        public async Task<IActionResult> Guncelle(int id, string? returnUrl)
        {
            var resp = await _http.GetAsync($"api/belge/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var belge = await resp.Content.ReadFromJsonAsync<dynamic>();
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? Url.Action("Details", "Staj", new { id = (int)belge!.stajID })
                : returnUrl;

            return View(belge);
        }

        // Güncelle (POST) — yeni dosyayı API'ye yükle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(int id, IFormFile yeniDosya, int stajId, string? returnUrl)
        {
            if (yeniDosya == null || yeniDosya.Length == 0)
            {
                TempData["Hata"] = "Dosya seçilmedi.";
                return RedirectToAction(nameof(Guncelle), new { id, returnUrl });
            }

            using var content = new MultipartFormDataContent();
            var stream = yeniDosya.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(yeniDosya.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "yeniDosya", yeniDosya.FileName);

            var resp = await _http.PutAsync($"api/belge/{id}/file", content);
            if (!resp.IsSuccessStatusCode)
            {
                TempData["Hata"] = "Güncelleme başarısız.";
                return RedirectToAction(nameof(Guncelle), new { id, returnUrl });
            }

            TempData["OK"] = "Belge güncellendi.";
            if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Details", "Staj", new { id = stajId });
        }
    }
}
