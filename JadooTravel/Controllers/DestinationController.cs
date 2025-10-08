

using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace JadooTravel.Controllers
{
    public class DestinationController : Controller
    {
        private readonly IDestinationService _destinationService;
        private readonly IMapper _mapper;

        public DestinationController(IDestinationService destinationService, IMapper mapper)
        {
            _destinationService = destinationService;
            _mapper = mapper;
        }

        public async Task<IActionResult> DestinationList()
        {
            var values = await _destinationService.TGetAllListAsync();
            var valueList = _mapper.Map<List<ResultDestinationDto>>(values);
            return View(valueList);
        }
        [HttpGet]
        public IActionResult CreateDestination()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDestination(CreateDestinationDto createDestinationDto)
        {
            var values = _mapper.Map<Destination>(createDestinationDto);
            await _destinationService.TCreateAsync(values);
            return RedirectToAction("DestinationList");
        }

        public async Task<IActionResult> DeleteDestination(ObjectId id)
        {
            await _destinationService.TDeleteAsync(id);
            return RedirectToAction("DestinationList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateDestination(ObjectId id)
        {
            var value = await _destinationService.TGetByIdAsync(id);
            var updateDestination = _mapper.Map<UpdateDestinationDto>(value);
            return View(updateDestination);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDestination(UpdateDestinationDto updateDestinationDto)
        {
            var values = _mapper.Map<Destination>(updateDestinationDto);
            await _destinationService.TUpdateAsync(values);
            return RedirectToAction("DestinationList");
        }
    }
}
