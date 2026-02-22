using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CtaDtos;
using JadooTravel.Dto.Dtos.FeatureDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    public class CtaController (ICtaService _ctaService): Controller
    {
        public async Task<IActionResult> CtaList()
        {
            var values = await _ctaService.GetAllAsync();
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateCta()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCta(CreateCtaDto  createCtaDto)
        {
            await _ctaService.CreateAsync(createCtaDto);
            return RedirectToAction("CtaList");
        }

        public async Task<IActionResult> DeleteCta(string id)
        {
            await _ctaService.DeleteAsync(id);
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
            return RedirectToAction("CtaListCta");
        }
    }
}
