using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface ICategoryService : IGenericService<ResultCategoryDto,CreateCategoryDto,UpdateCategoryDto>
    {
    }
}
