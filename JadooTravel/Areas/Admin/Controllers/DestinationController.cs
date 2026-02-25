

using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DestinationController : Controller
    {
        private readonly IDestinationService _destinationService;
        private readonly IElasticAuditLogger _auditLogger;
        public DestinationController(IDestinationService destinationService, IElasticAuditLogger auditLogger)
        {
            _destinationService = destinationService;
            _auditLogger = auditLogger;
        }

        public async Task<IActionResult> DestinationList()
        {
            var values = await _destinationService.GetAllAsync();
            await _auditLogger.LogAsync("admin.destination.list", "admin", User.Identity?.Name, "list", "destination", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateDestination() => View();
        [HttpPost]
        public async Task<IActionResult> CreateDestination(CreateDestinationDto createDestinationDto)
        {
            await _destinationService.CreateAsync(createDestinationDto);
            await _auditLogger.LogAsync("admin.destination.create", "admin", User.Identity?.Name, "create", "destination", null, "success");
            return RedirectToAction("DestinationList");
        }

        public async Task<IActionResult> DeleteDestination(string id)
        {
            await _destinationService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.destination.delete", "admin", User.Identity?.Name, "delete", "destination", id, "success");
            return RedirectToAction("DestinationList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateDestination(string id)
        {
            var value = await _destinationService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDestination(UpdateDestinationDto updateDestinationDto)
        {
            await _destinationService.UpdateAsync(updateDestinationDto);
            await _auditLogger.LogAsync("admin.destination.update", "admin", User.Identity?.Name, "update", "destination", updateDestinationDto.Id, "success");
            return RedirectToAction("DestinationList");
        }
    }
}
