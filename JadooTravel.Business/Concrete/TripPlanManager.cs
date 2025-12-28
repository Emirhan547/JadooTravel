using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class TripPlanManager (ITripPlanDal _tripPlanDal,IMapper _mapper): ITripPlanService
    {
        public async Task CreateAsync(CreateTripPlanDto create)
        {
            var mapped= _mapper.Map<TripPlan>(create);
            await _tripPlanDal.CreateAsync(mapped);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _tripPlanDal.DeleteAsync(id);
        }

        public async Task<List<ResultTripPlanDto>> GetAllAsync()
        {
            var tripPlans= await _tripPlanDal.GetAllAsync();
            return _mapper.Map<List<ResultTripPlanDto>>(tripPlans);
        }

        public async Task<ResultTripPlanDto> GetByIdAsync(ObjectId id)
        {
            var tripPlan=await _tripPlanDal.GetByIdAsync(id);
            return _mapper.Map<ResultTripPlanDto>(tripPlan);
        }

        public async Task UpdateAsync(UpdateTripPlanDto update)
        {
            var mapped=_mapper.Map<TripPlan>(update);
            await _tripPlanDal.UpdateAsync(mapped);
        }
    }
}
