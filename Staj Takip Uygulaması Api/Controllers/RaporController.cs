using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;

namespace StajTakipUygulaması.Controllers
{
    [Route("[controller]/[action]")]
    public class RaporController : Controller
    {
        private readonly IRaporService _raporService;
        private readonly StajContext _ctx; // Belge tipi dropdown için

        public RaporController(IRaporService raporService, StajContext ctx)
        {
            _raporService = raporService;
            _ctx = ctx;
        }

        // Arama formu (GET)
        [HttpGet]
        public async Task<IActionResult> Ara(int belgeTipId = 0, CancellationToken ct = default)
        {
            ViewBag.BelgeTipId = belgeTipId;
            ViewBag.BelgeTipleri = await _ctx.BelgeTipleri
                .AsNoTracking()
                .OrderBy(t => t.Ad)
                .Select(t => new SelectListItem { Value = t.ID.ToString(), Text = t.Ad })
                .ToListAsync(ct);

            return View(); // Views/Rapor/Ara.cshtml
        }

        // Arama sonucu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ara(string arama, int belgeTipId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(arama))
            {
                TempData["Hata"] = "Lütfen arama alanını doldurun.";
                return RedirectToAction(nameof(Ara), new { belgeTipId });
            }

            var sonuc = await _raporService.AraAsync(arama.Trim(), belgeTipId, ct);

            ViewBag.BelgeTipId = belgeTipId;
            ViewBag.BelgeTipleri = await _ctx.BelgeTipleri
                .AsNoTracking()
                .OrderBy(t => t.Ad)
                .Select(t => new SelectListItem { Value = t.ID.ToString(), Text = t.Ad })
                .ToListAsync(ct);

            return View("Sonuc", sonuc); // Views/Rapor/Sonuc.cshtml -> @model List<RaporStajyerDto>
        }
    }
}
