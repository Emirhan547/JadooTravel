using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.ViewComponents
{
    public class _DefaultTestimonialViewComponent:ViewComponent
    {
        private readonly ITestimonialService _testimonialService;
        private readonly IMapper _mapper;

        public _DefaultTestimonialViewComponent(ITestimonialService testimonialService, IMapper mapper)
        {
            _testimonialService = testimonialService;
            _mapper = mapper;
        }

        public async Task <IViewComponentResult> InvokeAsync()
        {
            var values = await _testimonialService.TGetAllListAsync();
            var result = _mapper.Map<List<ResultTestimonialDto>>(values);
            return View(result);
        }
    }
}
