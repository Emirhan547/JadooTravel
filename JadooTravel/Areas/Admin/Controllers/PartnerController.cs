using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CtaDtos;
using JadooTravel.Dto.Dtos.PartnerDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    public class PartnerController(IPartnerService _partnerService) : Controller
    {
        public async Task<IActionResult> PartnerList()
        {
            var values = await _partnerService.GetAllAsync();
            return View(values);
        }
        [HttpGet]
        public IActionResult CreatePartner()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePartner(CreatePartnerDto createPartnerDto)
        {
            await _partnerService.CreateAsync(createPartnerDto);
            return RedirectToAction("PartnerList");
        }

        public async Task<IActionResult> DeletePartner(string id)
        {
            await _partnerService.DeleteAsync(id);
            return RedirectToAction("PartnerList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdatePartner(string id)
        {
            var value = await _partnerService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePartner(UpdatePartnerDto updatePartnerDto)
        {
            await _partnerService.UpdateAsync(updatePartnerDto);
            return RedirectToAction("PartnerList");
        }
    }
}
