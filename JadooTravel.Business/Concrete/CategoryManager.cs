using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class CategoryManager(ICategoryDal _categoryDal,IMapper _mapper) : ICategoryService
    {
        public async Task CreateAsync(CreateCategoryDto create)
        {
            var mapped= _mapper.Map<Category>(create);
           await _categoryDal.CreateAsync(mapped);
        }

        public async Task DeleteAsync(ObjectId id)
        {
           await _categoryDal.DeleteAsync(id);
        }

        public async Task<List<ResultCategoryDto>> GetAllAsync()
        {
           var categories= await _categoryDal.GetAllAsync();
            return _mapper.Map<List<ResultCategoryDto>>(categories);
        }

        public async Task<ResultCategoryDto> GetByIdAsync(ObjectId id)
        {
            var category=await _categoryDal.GetByIdAsync(id);
            return await _mapper.Map<Task<ResultCategoryDto>>(category);
        }

        public async Task UpdateAsync(UpdateCategoryDto update)
        {
            var mapped=_mapper.Map<Category>(update);
            await _categoryDal.UpdateAsync(mapped);
        }
    }
}
