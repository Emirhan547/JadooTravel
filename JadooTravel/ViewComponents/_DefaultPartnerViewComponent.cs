using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents
{
    public class _DefaultPartnerViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
