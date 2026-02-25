using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FeatureController : Controller
    {
        private readonly IFeatureService _featureService;
        private readonly IElasticAuditLogger _auditLogger;
        public FeatureController(IFeatureService featureService, IMapper mapper, IElasticAuditLogger auditLogger)
        {
            _featureService = featureService;
            _auditLogger = auditLogger;
        }

        public async Task<IActionResult> FeatureList()
        {
            var values = await _featureService.GetAllAsync();
            await _auditLogger.LogAsync("admin.feature.list", "admin", User.Identity?.Name, "list", "feature", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateFeature() => View();
        [HttpPost]
        public async Task<IActionResult> CreateFeature(CreateFeatureDto createFeatureDto)
        {
            await _featureService.CreateAsync(createFeatureDto);
            await _auditLogger.LogAsync("admin.feature.create", "admin", User.Identity?.Name, "create", "feature", null, "success");
            return RedirectToAction("FeatureList");
        }

        public async Task<IActionResult> DeleteFeature(string id)
        {
            await _featureService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.feature.delete", "admin", User.Identity?.Name, "delete", "feature", id, "success");
            return RedirectToAction("FeatureList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateFeature(string id)
        {
            var value = await _featureService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFeature(UpdateFeatureDto updateFeatureDto)
        {
            await _featureService.UpdateAsync(updateFeatureDto);
            await _auditLogger.LogAsync("admin.feature.update", "admin", User.Identity?.Name, "update", "feature", updateFeatureDto.Id, "success");
            return RedirectToAction("FeatureList");
        }
    }
}
