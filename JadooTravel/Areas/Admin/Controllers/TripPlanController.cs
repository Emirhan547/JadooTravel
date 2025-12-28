using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TripPlanController : Controller
    {
        private readonly ITripPlanService _tripPlanService;

        public TripPlanController(ITripPlanService tripPlanService, IMapper mapper)
        {
            _tripPlanService = tripPlanService;

        }

        public async Task<IActionResult> TripPlanList()
        {
            var values = await _tripPlanService.GetAllAsync();
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateTripPlan()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTripPlan(CreateTripPlanDto createTripPlanDto)
        {
            await _tripPlanService.CreateAsync(createTripPlanDto);
            return RedirectToAction("TripPlanList");
        }

        public async Task<IActionResult> DeleteTripPlan(ObjectId id)
        {
            await _tripPlanService.DeleteAsync(id);
            return RedirectToAction("TripPlanList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTripPlan(ObjectId id)
        {
            var value = await _tripPlanService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFeature(UpdateTripPlanDto updateTripPlanDto)
        {
 
            await _tripPlanService.UpdateAsync(updateTripPlanDto);
            return RedirectToAction("TripPlanList");
        }
    }
}
