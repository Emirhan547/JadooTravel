using JadooTravel.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultPartnerViewComponent(IPartnerService _partnerService):ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _partnerService.GetAllAsync();
            return View(values);
        }
    }
}
