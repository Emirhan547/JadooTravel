using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultTestimonialViewComponent:ViewComponent
    {
        private readonly ITestimonialService _testimonialService;

        public _DefaultTestimonialViewComponent(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        public async Task <IViewComponentResult> InvokeAsync()
        {
            var values = await _testimonialService.GetAllAsync();
            return View(values);
        }
    }
}
