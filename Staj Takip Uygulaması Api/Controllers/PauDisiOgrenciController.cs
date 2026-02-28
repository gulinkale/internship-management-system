// Proje: StajTakipUygulamasi.Web
// Dosya: Controllers/PauDisiOgrenciController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using StajTakipUygulaması.Models; // OgrenciViewModel
using StajTakipUygulaması.Application.DTOs; // StajyerCreateDto, StajyerDto, StajCreateDto, StajDto
using StajTakipUygulaması.Domain.Entities; // Stajyer, Staj

namespace StajTakipUygulamasi.Web.Controllers
{
    public class PauDisiOgrenciController : Controller
    {
        private readonly HttpClient _http;

        private const string EP_STAJ_TURU = "api/StajTurleri";
        private const string EP_STAJYER = "api/v1/stajyerler";
        private const string EP_STAJ = "api/staj";
        private const string EP_BELGE = "api/belge/upload";

        public PauDisiOgrenciController(IHttpClientFactory f)
        {
            _http = f.CreateClient("Api");
        }

        // 🔍 Detay (UI sadece API'den çeker)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var resp = await _http.GetAsync($"{EP_STAJYER}/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var stajyer = await resp.Content.ReadFromJsonAsync<StajyerDto>();
            return View(stajyer);
        }

        // 📄 Form (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadStajTurleriAsync();
            if (TempData["Mesaj"] != null)
                ViewBag.Mesaj = TempData["Mesaj"];

            return View(new OgrenciViewModel());
        }

        // ======= HELPER: Staj türleri doldur =======
        private async Task LoadStajTurleriAsync()
        {
            try
            {
                var resp = await _http.GetAsync(EP_STAJ_TURU);
                if (!resp.IsSuccessStatusCode)
                {
                    ViewBag.StajTurleri = new List<SelectListItem>();
                    ModelState.AddModelError("", "Staj türleri yüklenemedi.");
                    return;
                }

                var turler = await resp.Content.ReadFromJsonAsync<List<StajTuruDto>>() ?? new List<StajTuruDto>();

                ViewBag.StajTurleri = turler
                    .Select(st => new SelectListItem { Value = st.Id.ToString(), Text = st.Ad })
                    .ToList();
            }
            catch (Exception ex)
            {
                ViewBag.StajTurleri = new List<SelectListItem>();
                ModelState.AddModelError("", "Staj türleri alınırken hata: " + ex.Message);
            }
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
                await LoadStajTurleriAsync();
                return View(model);
            }

            // 1) Stajyer kayıt (PAÜ dışı)
            var stajyerCreate = new StajyerCreateDto
            {
                Universite = model.Stajyer.Universite,
                OgrenciNo = model.Stajyer.OgrenciNo,
                Bolum = model.Stajyer.Bolum,
                Fakulte = model.Stajyer.Fakulte,
                BaslamaYili = model.Stajyer.BaslamaYili,
                Sinif = model.Stajyer.Sinif,
                PAU_ogrencisi_mi = false,
                Ad = model.Stajyer.Ad,
                Soyad = model.Stajyer.Soyad,
                TCKimlikNo = model.Stajyer.TCKimlikNo,
                DogumTarihi = model.Stajyer.DogumTarihi,
                Cinsiyet = model.Stajyer.Cinsiyet,
                TelNo = model.Stajyer.TelNo,
                Email = model.Stajyer.Email,
                Adres = model.Stajyer.Adres
            };

            var stajyerResp = await _http.PostAsJsonAsync(EP_STAJYER, stajyerCreate);
            if (!stajyerResp.IsSuccessStatusCode)
            {
                await LoadStajTurleriAsync();
                ModelState.AddModelError("", "Stajyer kaydı başarısız.");
                return View(model);
            }

            var stajyer = await stajyerResp.Content.ReadFromJsonAsync<StajyerDto>();
            if (stajyer is null)
            {
                await LoadStajTurleriAsync();
                ModelState.AddModelError("", "Stajyer yanıtı okunamadı.");
                return View(model);
            }

            // 2) Staj kayıt
            var stajCreate = new StajCreateDto
            {
                StajyerID = stajyer.ID,
                StajTuruID = model.Staj.StajTuruID,
                BaslamaTarihi = model.Staj.BaslamaTarihi,
                BitisTarihi = model.Staj.BitisTarihi,
                Departman = model.Staj.Departman,
                SorumluID = model.Staj.SorumluID,
                Yetkiler = model.Staj.Yetkiler
            };

            var stajResp = await _http.PostAsJsonAsync(EP_STAJ, stajCreate);
            if (!stajResp.IsSuccessStatusCode)
            {
                await LoadStajTurleriAsync();
                ModelState.AddModelError("", "Staj kaydı başarısız.");
                return View(model);
            }

            var stajObj = await stajResp.Content.ReadFromJsonAsync<StajDto>();
            if (stajObj is null)
            {
                await LoadStajTurleriAsync();
                ModelState.AddModelError("", "Staj yanıtı okunamadı.");
                return View(model);
            }

            int stajId = stajObj.Id;

            // 3) Belgeler
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
                if (file is null || file.Length == 0) continue;

                await using var stream = file.OpenReadStream();
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                content.Add(fileContent, "dosya", file.FileName);
                content.Add(new StringContent(stajId.ToString()), "stajId");
                content.Add(new StringContent(belgeTipiId.ToString()), "belgeTipId");
                content.Add(new StringContent(ad), "aciklama");

                var uploadResp = await _http.PostAsync(EP_BELGE, content);
                if (!uploadResp.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", $"Belge yükleme başarısız: {ad}");
                    // Devam etmek istiyorsan kesmeden ilerle (continue)
                }
            }

            TempData["Mesaj"] = "✅ PAÜ Dışı Öğrenci başarıyla kaydedildi.";
            ModelState.Clear();
            await LoadStajTurleriAsync();
            return View(new OgrenciViewModel());
        }
    }

    // ---- Basit DTO'lar (varsa projenizde zaten mevcut) ----
    public sealed class StajTuruDto { public int Id { get; set; } public string Ad { get; set; } = ""; }
    public sealed class StajDto { public int Id { get; set; } }
}
