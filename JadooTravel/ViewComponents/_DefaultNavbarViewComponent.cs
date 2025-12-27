using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.ViewComponents
{
    public class _DefaultNavbarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
            { 
            return View();
             }
    }
}
