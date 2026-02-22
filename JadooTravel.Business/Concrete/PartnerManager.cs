using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.PartnerDtos;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class PartnerManager (IPartnerDal _partnerDal,IMapper _mapper): IPartnerService
    {
        public async Task CreateAsync(CreatePartnerDto create)
        {
            var mapped = _mapper.Map<Partner>(create);
            await _partnerDal.CreateAsync(mapped);
        }

        public async Task DeleteAsync(string id)
        {
            await _partnerDal.DeleteAsync(id);
        }

        public async Task<List<ResultPartnerDto>> GetAllAsync()
        {
            var feature = await _partnerDal.GetAllAsync();
            return _mapper.Map<List<ResultPartnerDto>>(feature);
        }

        public async Task<UpdatePartnerDto> GetByIdAsync(string id)
        {
            var features = await _partnerDal.GetByIdAsync(id);
            return _mapper.Map<UpdatePartnerDto>(features);
        }

        public async Task UpdateAsync(UpdatePartnerDto update)
        {
            var mapped = _mapper.Map<Partner>(update);
            await _partnerDal.UpdateAsync(mapped);
        }
    }
}
