using AutoMapper;
using Elastic.Clients.Elasticsearch;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TripPlanController : Controller
    {
        private readonly ITripPlanService _tripPlanService;
        private readonly IElasticAuditLogger _auditLogger;
        public TripPlanController(ITripPlanService tripPlanService, IElasticAuditLogger auditLogger)
        {
            _tripPlanService = tripPlanService;
            _auditLogger = auditLogger;
        }

        public async Task<IActionResult> TripPlanList()
        {
            var values = await _tripPlanService.GetAllAsync();
            await _auditLogger.LogAsync("admin.tripplan.list", "admin", User.Identity?.Name, "list", "tripplan", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateTripPlan() => View();
        [HttpPost]
        public async Task<IActionResult> CreateTripPlan(CreateTripPlanDto createTripPlanDto)
        {
            await _tripPlanService.CreateAsync(createTripPlanDto);
            await _auditLogger.LogAsync("admin.tripplan.create", "admin", User.Identity?.Name, "create", "tripplan", null, "success");
            return RedirectToAction("TripPlanList");
        }

        public async Task<IActionResult> DeleteTripPlan(string id)
        {
            await _tripPlanService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.tripplan.delete", "admin", User.Identity?.Name, "delete", "tripplan", id, "success");
            return RedirectToAction("TripPlanList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTripPlan(string id)
        {
            var value = await _tripPlanService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFeature(UpdateTripPlanDto updateTripPlanDto)
        {
 
            await _tripPlanService.UpdateAsync(updateTripPlanDto);
            await _auditLogger.LogAsync("admin.tripplan.update", "admin", User.Identity?.Name, "update", "tripplan", updateTripPlanDto.Id, "success");
            return RedirectToAction("TripPlanList");
        }
    }
}
