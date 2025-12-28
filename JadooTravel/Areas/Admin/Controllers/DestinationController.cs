

using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DestinationController : Controller
    {
        private readonly IDestinationService _destinationService;

        public DestinationController(IDestinationService destinationService, IMapper mapper)
        {
            _destinationService = destinationService;
        }

        public async Task<IActionResult> DestinationList()
        {
            var values = await _destinationService.GetAllAsync();
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateDestination()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDestination(CreateDestinationDto createDestinationDto)
        {
            await _destinationService.CreateAsync(createDestinationDto);
            return RedirectToAction("DestinationList");
        }

        public async Task<IActionResult> DeleteDestination(ObjectId id)
        {
            await _destinationService.DeleteAsync(id);
            return RedirectToAction("DestinationList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateDestination(ObjectId id)
        {
            var value = await _destinationService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDestination(UpdateDestinationDto updateDestinationDto)
        {
            await _destinationService.UpdateAsync(updateDestinationDto);
            return RedirectToAction("DestinationList");
        }
    }
}
