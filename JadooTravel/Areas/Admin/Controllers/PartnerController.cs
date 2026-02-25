using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CtaDtos;
using JadooTravel.Dto.Dtos.PartnerDtos;
using JadooTravel.UI.Logging;
using JadooTravel.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PartnerController(IPartnerService _partnerService, IElasticAuditLogger _auditLogger, IAwsS3Service _awsS3Service) : Controller
    {
        public async Task<IActionResult> PartnerList()
        {
            var values = await _partnerService.GetAllAsync();
            await _auditLogger.LogAsync("admin.partner.list", "admin", User.Identity?.Name, "list", "partner", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreatePartner() => View();
        [HttpPost]
        public async Task<IActionResult> CreatePartner(CreatePartnerDto createPartnerDto, IFormFile? imageFile)
        {
            if (imageFile is not null)
                createPartnerDto.ImageUrl = await _awsS3Service.UploadImageAsync(imageFile, "partners");
            await _partnerService.CreateAsync(createPartnerDto);
            await _auditLogger.LogAsync("admin.partner.create", "admin", User.Identity?.Name, "create", "partner", null, "success");
            return RedirectToAction("PartnerList");
        }

        public async Task<IActionResult> DeletePartner(string id)
        {
            await _partnerService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.partner.delete", "admin", User.Identity?.Name, "delete", "partner", id, "success");
            return RedirectToAction("PartnerList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdatePartner(string id)
        {
            var value = await _partnerService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePartner(UpdatePartnerDto updatePartnerDto, IFormFile? imageFile)
        {
            if (imageFile is not null)
                updatePartnerDto.ImageUrl = await _awsS3Service.UploadImageAsync(imageFile, "partners");
            await _partnerService.UpdateAsync(updatePartnerDto);
            await _auditLogger.LogAsync("admin.partner.update", "admin", User.Identity?.Name, "update", "partner", updatePartnerDto.Id, "success");
            return RedirectToAction("PartnerList");
        }
    }
}
