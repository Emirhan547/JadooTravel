using JadooTravel.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultCtaViewComponent(ICtaService _ctaService):ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _ctaService.GetAllAsync();
            var value = values.FirstOrDefault();
            return View(value);
        }
    }
}
