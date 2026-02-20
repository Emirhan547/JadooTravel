using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Admin
{
    public class _AdminLayoutFooterViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
