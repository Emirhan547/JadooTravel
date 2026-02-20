using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultBookingStepViewComponent:ViewComponent
    {
        private readonly ITripPlanService _tripPlanService;

        public _DefaultBookingStepViewComponent(ITripPlanService tripPlanService)
        {
            _tripPlanService = tripPlanService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var values = await _tripPlanService.GetAllAsync();
            return View(values);
        }
    }
}
