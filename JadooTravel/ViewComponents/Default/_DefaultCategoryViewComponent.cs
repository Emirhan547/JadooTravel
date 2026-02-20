using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CategoryDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.ViewComponents.Default
{
    public class _DefaultCategoryViewComponent:ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public _DefaultCategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _categoryService.GetAllAsync();
            return View(values);
        }
    }
}
