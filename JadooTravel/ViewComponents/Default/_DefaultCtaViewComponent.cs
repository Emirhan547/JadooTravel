using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultCtaViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
