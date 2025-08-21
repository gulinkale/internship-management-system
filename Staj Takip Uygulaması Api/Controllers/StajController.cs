// Proje: StajTakipUygulaması.Web
// Dosya: Controllers/StajController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using StajTakipUygulaması.Models;
using StajTakipUygulaması.Domain.Entities;

namespace StajTakipUygulaması.Web.Controllers
{
    public class StajController : Controller
    {
        private readonly HttpClient _http;
        public StajController(IHttpClientFactory f) => _http = f.CreateClient("Api");

        // GET /Staj
        public async Task<IActionResult> Index()
        {
            var resp = await _http.GetAsync("api/staj");
            if (!resp.IsSuccessStatusCode) return View(new List<Staj>());
            var data = await resp.Content.ReadFromJsonAsync<List<Staj>>();
            return View(data!);
        }

        // POST /Staj/Create   (formdan Staj gelir)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staj staj)
        {
            if (!ModelState.IsValid) return View(staj);

            var resp = await _http.PostAsJsonAsync("api/staj", staj);
            if (!resp.IsSuccessStatusCode)
            {
                TempData["Hata"] = "Staj kaydı başarısız.";
                return View(staj);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET /Staj/Aktif
        public async Task<IActionResult> Aktif()
        {
            var resp = await _http.GetAsync("api/staj/aktif");
            var data = resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<Staj>>() : new List<Staj>();
            return View("StajyerListesi/Aktif", data!);
        }

        // GET /Staj/Tamamlanmıs
        public async Task<IActionResult> Tamamlanmıs()
        {
            var resp = await _http.GetAsync("api/staj/tamamlanmis");
            var data = resp.IsSuccessStatusCode
                ? await resp.Content.ReadFromJsonAsync<List<Staj>>() : new List<Staj>();
            return View("StajyerListesi/Tamamlanmıs", data!);
        }

        // GET /Staj/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var stajResp = await _http.GetAsync($"api/staj/{id}");
            if (!stajResp.IsSuccessStatusCode) return NotFound();
            var staj = await stajResp.Content.ReadFromJsonAsync<Staj>();

            // Belge tipleri (dropdown vb.)
            var tipResp = await _http.GetAsync("api/belgetipi");
            var tipler = tipResp.IsSuccessStatusCode
                ? await tipResp.Content.ReadFromJsonAsync<List<dynamic>>() : new List<dynamic>();
            ViewBag.BelgeTipleri = tipler;

            // Eski view yolunu koruyalım:
            return View("~/Views/Staj/StajyerListesi/StajyerBilgileri.cshtml", staj!);
        }
    }
}
