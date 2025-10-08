using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly ITestimonialService _testimonialService;
        private readonly IMapper _mapper;

        public TestimonialController(ITestimonialService testimonialService, IMapper mapper)
        {
            _testimonialService = testimonialService;
            _mapper = mapper;
        }

        public async Task<IActionResult> TestimonialList()
        {
            var values = await _testimonialService.TGetAllListAsync();
            var valueList = _mapper.Map<List<ResultTestimonialDto>>(values);
            return View(valueList);
        }
        [HttpGet]
        public IActionResult CreateTestimonial()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTestimonial(CreateTestimonialDto createTestimonialDto)
        {
            var values = _mapper.Map<Testimonial>(createTestimonialDto);
            await _testimonialService.TCreateAsync(values);
            return RedirectToAction("TestimonialList");
        }

        public async Task<IActionResult> DeleteTestimonial(ObjectId id)
        {
            await _testimonialService.TDeleteAsync(id);
            return RedirectToAction("TestimonialList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTestimonial(ObjectId id)
        {
            var value = await _testimonialService.TGetByIdAsync(id);
            var updateTestimonial = _mapper.Map<UpdateTestimonialDto>(value);
            return View(updateTestimonial);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTestimonial(UpdateTestimonialDto updateTestimonialDto)
        {
            var values = _mapper.Map<Testimonial>(updateTestimonialDto);
            await _testimonialService.TUpdateAsync(values);
            return RedirectToAction("TestimonialList");
        }
    }
}
