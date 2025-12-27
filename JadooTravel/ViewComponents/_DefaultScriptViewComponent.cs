using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents
{
    public class _DefaultScriptViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
