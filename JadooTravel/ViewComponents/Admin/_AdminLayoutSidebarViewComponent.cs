using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Admin
{
    public class _AdminLayoutSidebarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
   
    }
}
