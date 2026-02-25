using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CtaDtos;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CtaController(ICtaService _ctaService, IElasticAuditLogger _auditLogger) : Controller
    {
        public async Task<IActionResult> CtaList()
        {
            var values = await _ctaService.GetAllAsync();
            await _auditLogger.LogAsync("admin.cta.list", "admin", User.Identity?.Name, "list", "cta", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateCta() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCta(CreateCtaDto createCtaDto)
        {
            await _ctaService.CreateAsync(createCtaDto);
            await _auditLogger.LogAsync("admin.cta.create", "admin", User.Identity?.Name, "create", "cta", null, "success");
            return RedirectToAction("CtaList");
        }

        public async Task<IActionResult> DeleteCta(string id)
        {
            await _ctaService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.cta.delete", "admin", User.Identity?.Name, "delete", "cta", id, "success");
            return RedirectToAction("CtaList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCta(string id)
        {
            var value = await _ctaService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCta(UpdateCtaDto updateCtaDto)
        {
            await _ctaService.UpdateAsync(updateCtaDto);
            await _auditLogger.LogAsync("admin.cta.update", "admin", User.Identity?.Name, "update", "cta", updateCtaDto.Id, "success");
            return RedirectToAction("CtaList");
        }
    }
}
