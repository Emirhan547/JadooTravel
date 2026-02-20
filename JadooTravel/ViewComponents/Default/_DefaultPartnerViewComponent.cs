using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultPartnerViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
