using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents
{
    public class _DefaultFooterViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
