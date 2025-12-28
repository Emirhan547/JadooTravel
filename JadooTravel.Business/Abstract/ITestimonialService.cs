using JadooTravel.Dto.Dtos.TestimonialDtos;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface ITestimonialService : IGenericService<ResultTestimonialDto,CreateTestimonialDto,UpdateTestimonialDto>
    {
    }
}
