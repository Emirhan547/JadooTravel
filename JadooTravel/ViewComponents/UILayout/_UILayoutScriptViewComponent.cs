using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.UILayout
{
    public class _UILayoutScriptViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
