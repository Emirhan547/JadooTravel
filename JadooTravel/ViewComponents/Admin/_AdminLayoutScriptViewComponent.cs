using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Admin
{
    public class _AdminLayoutScriptViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}