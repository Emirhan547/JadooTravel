using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultBookingStepViewComponent:ViewComponent
    {
        private readonly ITripPlanService _tripPlanService;
        private readonly IDestinationService _destinationService;
        public _DefaultBookingStepViewComponent(
             ITripPlanService tripPlanService,
             IDestinationService destinationService)
        {
            _tripPlanService = tripPlanService;
            _destinationService = destinationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Destinations = await _destinationService.GetAllAsync();
            var values = await _tripPlanService.GetAllAsync();
            return View(values);
        }
    }
}
