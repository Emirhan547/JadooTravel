using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.UILayout
{
    public class _UILayoutNavbarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
            { 
            return View();
             }
    }
}
