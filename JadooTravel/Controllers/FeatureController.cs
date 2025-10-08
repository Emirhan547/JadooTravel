using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IFeatureService _featureService;
        private readonly IMapper _mapper;

        public FeatureController(IFeatureService featureService, IMapper mapper)
        {
            _featureService = featureService;
            _mapper = mapper;
        }

        public async Task<IActionResult> FeatureList()
        {
            var values = await _featureService.TGetAllListAsync();
            var valueList = _mapper.Map<List<ResultFeatureDto>>(values);
            return View(valueList);
        }
        [HttpGet]
        public IActionResult CreateFeature()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateFeature(CreateFeatureDto createFeatureDto)
        {
            var values = _mapper.Map<Feature>(createFeatureDto);
            await _featureService.TCreateAsync(values);
            return RedirectToAction("FeatureList");
        }

        public async Task<IActionResult> DeleteFeature(ObjectId id)
        {
            await _featureService.TDeleteAsync(id);
            return RedirectToAction("FeatureList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateFeature(ObjectId id)
        {
            var value = await _featureService.TGetByIdAsync(id);
            var updateFeature = _mapper.Map<UpdateFeatureDto>(value);
            return View(updateFeature);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFeature(UpdateFeatureDto updateFeatureDto)
        {
            var values = _mapper.Map<Feature>(updateFeatureDto);
            await _featureService.TUpdateAsync(values);
            return RedirectToAction("FeatureList");
        }
    }
}
