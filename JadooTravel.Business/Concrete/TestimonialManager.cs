using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class TestimonialManager(ITestimonialDal _testimonialDal, IMapper _mapper) : ITestimonialService
    {
        public async Task CreateAsync(CreateTestimonialDto create)
        {
            var mapped=_mapper.Map<Testimonial>(create);
            await _testimonialDal.CreateAsync(mapped);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _testimonialDal.DeleteAsync(id);
        }

        public async Task<List<ResultTestimonialDto>> GetAllAsync()
        {
            var testtimonials=await _testimonialDal.GetAllAsync();
            return _mapper.Map<List<ResultTestimonialDto>>(testtimonials);
        }

        public async Task<ResultTestimonialDto> GetByIdAsync(ObjectId id)
        {
            var testimonials=await _testimonialDal.GetByIdAsync(id);
            return _mapper.Map<ResultTestimonialDto>(testimonials);
        }

        public async Task UpdateAsync(UpdateTestimonialDto update)
        {
            var mapped=_mapper.Map<Testimonial>(update);
            await _testimonialDal.UpdateAsync(mapped);
        }
    }
}
