
using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IElasticAuditLogger _auditLogger;

        public CategoryController(ICategoryService categoryService, IElasticAuditLogger auditLogger)
        {
            _categoryService = categoryService;
            _auditLogger = auditLogger;
        }

        public async Task<IActionResult> CategoryList()
        {
            var values = await _categoryService.GetAllAsync();
            await _auditLogger.LogAsync("admin.category.list", "admin", User.Identity?.Name, "list", "category", null, "success", new { count = values.Count });
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateCategory() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            await _categoryService.CreateAsync(createCategoryDto);
            await _auditLogger.LogAsync("admin.category.create", "admin", User.Identity?.Name, "create", "category", null, "success");
            return RedirectToAction("CategoryList");
        }
        
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteAsync(id);
            await _auditLogger.LogAsync("admin.category.delete", "admin", User.Identity?.Name, "delete", "category", id, "success");
            return RedirectToAction("CategoryList");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(string id)
        {
            var value = await _categoryService.GetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            await _categoryService.UpdateAsync(updateCategoryDto);
            await _auditLogger.LogAsync("admin.category.update", "admin", User.Identity?.Name, "update", "category", updateCategoryDto.Id, "success");
            return RedirectToAction("CategoryList");
        }
    }
}
