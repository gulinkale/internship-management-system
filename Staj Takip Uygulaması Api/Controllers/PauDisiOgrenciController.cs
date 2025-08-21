// Proje: StajTakipUygulamasi.Web
// Dosya: Controllers/PauDisiOgrenciController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using StajTakipUygulaması.Models; // OgrenciViewModel için (varsa)
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Domain.Entities; // StajyerCreateDto vs.

namespace StajTakipUygulamasi.Web.Controllers
{
    public class PauDisiOgrenciController : Controller
    {
        private readonly HttpClient _http;
        public PauDisiOgrenciController(IHttpClientFactory f) => _http = f.CreateClient("Api");

        // 🔍 Detay (UI sadece API'den çeker)
        public async Task<IActionResult> Details(int id)
        {
            var resp = await _http.GetAsync($"api/stajyer/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var stajyer = await resp.Content.ReadFromJsonAsync<StajyerDto>();
            return View(stajyer);
        }

        // 📄 Form (GET)
        public async Task<IActionResult> Create()
        {
            // Staj türleri dropdown (Api'den)
            var turlerResp = await _http.GetAsync("api/stajturu");
            var turler = turlerResp.IsSuccessStatusCode
                ? await turlerResp.Content.ReadFromJsonAsync<List<dynamic>>()
                : new List<dynamic>();

            ViewBag.StajTurleri = (turler ?? new List<dynamic>()).Select(st => new SelectListItem
            {
                Value = ((int)st.id).ToString(),
                Text = (string)st.ad
            }).ToList();

            if (TempData["Mesaj"] != null)
                ViewBag.Mesaj = TempData["Mesaj"];

            return View(new OgrenciViewModel());
        }

        // 📝 Form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OgrenciViewModel model)
        {
            model.Stajyer ??= new Stajyer();
            model.Staj ??= new Staj();

            if (!ModelState.IsValid)
            {
                // Dropdown'ı yeniden doldur
                var turlerResp = await _http.GetAsync("api/stajturu");
                var turler = turlerResp.IsSuccessStatusCode
                    ? await turlerResp.Content.ReadFromJsonAsync<List<dynamic>>()
                    : new List<dynamic>();

                ViewBag.StajTurleri = (turler ?? new List<dynamic>()).Select(st => new SelectListItem
                {
                    Value = ((int)st.id).ToString(),
                    Text = (string)st.ad
                }).ToList();

                return View(model);
            }

            // 1) Stajyer kayıt (API)
            var createDto = new StajyerCreateDto
            {
                Universite = model.Stajyer.Universite,
                OgrenciNo = model.Stajyer.OgrenciNo,
                Bolum = model.Stajyer.Bolum,
                Fakulte = model.Stajyer.Fakulte,
                BaslamaYili = model.Stajyer.BaslamaYili,
                Sinif = model.Stajyer.Sinif,
                PAU_ogrencisi_mi = false, // PAÜ dışı
                Ad = model.Stajyer.Ad,
                Soyad = model.Stajyer.Soyad,
                TCKimlikNo = model.Stajyer.TCKimlikNo,
                DogumTarihi = model.Stajyer.DogumTarihi,
                Cinsiyet = model.Stajyer.Cinsiyet,
                TelNo = model.Stajyer.TelNo,
                Email = model.Stajyer.Email,
                Adres = model.Stajyer.Adres
            };

            var stajyerResp = await _http.PostAsJsonAsync("api/stajyer", createDto);
            if (!stajyerResp.IsSuccessStatusCode)
            {
                TempData["Mesaj"] = "Stajyer kaydı başarısız.";
                return RedirectToAction(nameof(Create));
            }

            var stajyer = await stajyerResp.Content.ReadFromJsonAsync<StajyerDto>();
            var stajyerId = stajyer!.ID;

            // 2) Staj kayıt (API)
            var stajCreate = new
            {
                StajyerID = stajyerId,
                StajTuruID = model.Staj.StajTuruID,
                BaslamaTarihi = model.Staj.BaslamaTarihi,
                BitisTarihi = model.Staj.BitisTarihi,
                Departman = model.Staj.Departman,
                SorumluID = model.Staj.SorumluID,
                Yetkiler = model.Staj.Yetkiler
            };

            var stajResp = await _http.PostAsJsonAsync("api/staj", stajCreate);
            if (!stajResp.IsSuccessStatusCode)
            {
                TempData["Mesaj"] = "Staj kaydı başarısız.";
                return RedirectToAction(nameof(Create));
            }
            var stajObj = await stajResp.Content.ReadFromJsonAsync<dynamic>();
            int stajId = (int)stajObj!.id;

            // 3) Belgeler (her dosya için /api/belge/upload)
            var belgeler = new List<(IFormFile? file, string ad, int belgeTipId)>
            {
                (model.OgrenciBelgesi, "Öğrenci Belgesi", 1),
                (model.Transkript,     "Transkript",       2),
                (model.BasvuruFormu,   "Başvuru Formu",    3),
                (model.Taahutname,     "Taahhütname",      4),
                (model.Referans,       "Referans Mektubu", 5)
            };

            foreach (var (file, ad, belgeTipiId) in belgeler)
            {
                if (file is null) continue;

                using var content = new MultipartFormDataContent();
                var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                content.Add(fileContent, "dosya", file.FileName);
                content.Add(new StringContent(stajId.ToString()), "stajId");
                content.Add(new StringContent(belgeTipiId.ToString()), "belgeTipId");
                content.Add(new StringContent(""), "aciklama");

                var uploadResp = await _http.PostAsync("api/belge/upload", content);
                if (!uploadResp.IsSuccessStatusCode)
                {
                    TempData["Mesaj"] = $"Belge yükleme başarısız: {ad}";
                    // hata olsa bile diğerlerine devam etmek istersen burada continue;
                    // yoksa return RedirectToAction(...) deyip kesebilirsin
                }
            }

            ViewBag.Mesaj = "PAÜ Dışı Öğrenci başarıyla kaydedildi.";
            ModelState.Clear();
            return View(new OgrenciViewModel());
        }
    }
}
