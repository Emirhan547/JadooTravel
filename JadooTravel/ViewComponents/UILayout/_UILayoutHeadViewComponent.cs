using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.UILayout
{
    public class _UILayoutHeadViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
