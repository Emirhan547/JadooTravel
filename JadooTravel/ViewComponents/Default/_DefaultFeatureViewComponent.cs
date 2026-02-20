using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultFeatureViewComponent:ViewComponent
    {
        private readonly IFeatureService _featureService;

        public _DefaultFeatureViewComponent(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _featureService.GetAllAsync();
            return View(values);
        }
    }
}
