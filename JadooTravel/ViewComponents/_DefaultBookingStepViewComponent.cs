using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.ViewComponents
{
    public class _DefaultBookingStepViewComponent:ViewComponent
    {
        private readonly ITripPlanService _tripPlanService;
        private readonly IMapper _mapper;

        public _DefaultBookingStepViewComponent(ITripPlanService tripPlanService, IMapper mapper)
        {
            _tripPlanService = tripPlanService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var values = await _tripPlanService.TGetAllListAsync();
            var result = _mapper.Map<List<ResultTripPlanDto>>(values);
            return View(result);
        }
    }
}
