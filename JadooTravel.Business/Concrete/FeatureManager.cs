using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class FeatureManager(IFeatureDal _featureDal ,IMapper _mapper) : IFeatureService
    {
        public async Task CreateAsync(CreateFeatureDto create)
        {
            var mapped=_mapper.Map<Feature>(create);
            await _featureDal.CreateAsync(mapped);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _featureDal.DeleteAsync(id);
        }

        public async Task<List<ResultFeatureDto>> GetAllAsync()
        {
            var feature=await _featureDal.GetAllAsync();
            return _mapper.Map<List<ResultFeatureDto>>(feature);
        }

        public async Task<ResultFeatureDto> GetByIdAsync(ObjectId id)
        {
            var features=await _featureDal.GetByIdAsync(id);
            return _mapper.Map<ResultFeatureDto>(features);
        }

        public async Task UpdateAsync(UpdateFeatureDto update)
        {
            var mapped=_mapper.Map<Feature>(update);
            await _featureDal.UpdateAsync(mapped);
        }
    }
}
