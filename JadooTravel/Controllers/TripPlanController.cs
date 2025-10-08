using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Controllers
{
    public class TripPlanController : Controller
    {
        private readonly ITripPlanService _tripPlanService;
        private readonly IMapper _mapper;

        public TripPlanController(ITripPlanService tripPlanService, IMapper mapper)
        {
            _tripPlanService = tripPlanService;
            _mapper = mapper;
        }

        public async Task<IActionResult> TripPlanList()
        {
            var values = await _tripPlanService.TGetAllListAsync();
            var valueList = _mapper.Map<List<ResultTripPlanDto>>(values);
            return View(valueList);
        }
        [HttpGet]
        public IActionResult CreateTripPlan()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTripPlan(CreateTripPlanDto createTripPlanDto)
        {
            var values = _mapper.Map<TripPlan>(createTripPlanDto);
            await _tripPlanService.TCreateAsync(values);
            return RedirectToAction("TripPlanList");
        }

        public async Task<IActionResult> DeleteTripPlan(ObjectId id)
        {
            await _tripPlanService.TDeleteAsync(id);
            return RedirectToAction("TripPlanList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTripPlan(ObjectId id)
        {
            var value = await _tripPlanService.TGetByIdAsync(id);
            var updateTripPlan = _mapper.Map<UpdateTripPlanDto>(value);
            return View(updateTripPlan);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFeature(UpdateTripPlanDto updateTripPlanDto)
        {
            var values = _mapper.Map<TripPlan>(updateTripPlanDto);
            await _tripPlanService.TUpdateAsync(values);
            return RedirectToAction("TripPlanList");
        }
    }
}
