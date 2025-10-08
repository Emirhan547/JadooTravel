
using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace JadooTravel.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> CategoryList()
        {
            var values = await _categoryService.TGetAllListAsync();
            var valueList= _mapper.Map<List<ResultCategoryDto>>(values);    
            return View(valueList);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var values = _mapper.Map<Category>(createCategoryDto);
            await _categoryService.TCreateAsync(values);
            return RedirectToAction("CategoryList");
        }
        
        public async Task<IActionResult> DeleteCategory(ObjectId id)
        {
            await _categoryService.TDeleteAsync(id);
            return RedirectToAction("CategoryList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(ObjectId id)
        {
            var value = await _categoryService.TGetByIdAsync(id);
            var updateCategory = _mapper.Map<UpdateCategoryDto>(value);
            return View(updateCategory);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var values = _mapper.Map<Category>(updateCategoryDto);
            await _categoryService.TUpdateAsync(values);
            return RedirectToAction("CategoryList");
        }
    }
}
