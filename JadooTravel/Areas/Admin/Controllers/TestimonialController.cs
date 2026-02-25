using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using JadooTravel.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TestimonialController : Controller
    {
        private readonly ITestimonialService _testimonialService;
        private readonly IElasticAuditLogger _auditLogger;
        private readonly IAwsS3Service _awsS3Service;
        public TestimonialController(ITestimonialService testimonialService, IElasticAuditLogger auditLogger, IAwsS3Service awsS3Service)
        {
            _testimonialService = testimonialService;
            _auditLogger = auditLogger;
            _awsS3Service = awsS3Service;
        }

        public async Task<IActionResult> TestimonialList()
        {
            var values = await _testimonialService.GetAllAsync();
            await _auditLogger.LogAsync("admin.testimonial.list", "admin", User.Identity?.Name, "list", "testimonial", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateTestimonial() => View();
        [HttpPost]
        public async Task<IActionResult> CreateTestimonial(CreateTestimonialDto createTestimonialDto, IFormFile? imageFile)
        {
            if (imageFile is not null)
                createTestimonialDto.ImageUrl = await _awsS3Service.UploadImageAsync(imageFile, "testimonials");
            await _testimonialService.CreateAsync(createTestimonialDto);
            await _auditLogger.LogAsync("admin.testimonial.create", "admin", User.Identity?.Name, "create", "testimonial", null, "success");
            return RedirectToAction("TestimonialList");
        }

        public async Task<IActionResult> DeleteTestimonial(string id)
        {
            await _testimonialService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.testimonial.delete", "admin", User.Identity?.Name, "delete", "testimonial", id, "success");
            return RedirectToAction("TestimonialList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTestimonial(string id)
        {
            var value = await _testimonialService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTestimonial(UpdateTestimonialDto updateTestimonialDto, IFormFile? imageFile)
        {
            if (imageFile is not null)
                updateTestimonialDto.ImageUrl = await _awsS3Service.UploadImageAsync(imageFile, "testimonials");
            await _testimonialService.UpdateAsync(updateTestimonialDto);
            await _auditLogger.LogAsync("admin.testimonial.update", "admin", User.Identity?.Name, "update", "testimonial", updateTestimonialDto.Id, "success");
            return RedirectToAction("TestimonialList");
        }
    }
}
