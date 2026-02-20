using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Admin
{
    public class _AdminLayoutTopbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();

        }
    }
}
