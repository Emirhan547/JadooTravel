using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents
{
    public class _DefaultAIRecommendationsComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
