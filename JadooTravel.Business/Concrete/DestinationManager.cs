using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class DestinationManager(IDestinationDal _destinationDal,IMapper _mapper) : IDestinationService
    {
        public async Task CreateAsync(CreateDestinationDto create)
        {
            var maped=_mapper.Map<Destination>(create);
            await _destinationDal.CreateAsync(maped);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _destinationDal.DeleteAsync(id);
        }

        public async Task<List<ResultDestinationDto>> GetAllAsync()
        {
            var categories=await _destinationDal.GetAllAsync();
            return _mapper.Map<List<ResultDestinationDto>>(categories);
        }

        public async Task<UpdateDestinationDto> GetByIdAsync(ObjectId id)
        {
            var destination=await _destinationDal.GetByIdAsync(id);
            return _mapper.Map<UpdateDestinationDto>(destination);
        }

        public async Task UpdateAsync(UpdateDestinationDto update)
        {
            var mapped = _mapper.Map<Destination>(update);
            await _destinationDal.UpdateAsync(mapped);
        }
    }
}
