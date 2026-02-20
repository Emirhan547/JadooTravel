

using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DestinationDtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultDestinationViewComponent:ViewComponent
    {
        private readonly IDestinationService _destinationService;

        public _DefaultDestinationViewComponent(IDestinationService destinationService)
        {
            _destinationService = destinationService;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values=await _destinationService.GetAllAsync();
            return View(values);
        }
    }
}
