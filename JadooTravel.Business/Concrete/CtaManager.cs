using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.CtaDtos;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class CtaManager(IMapper _mapper,ICtaDal _ctaDal) : ICtaService
    {
        public async Task CreateAsync(CreateCtaDto create)
        {
            var mapped = _mapper.Map<Cta>(create);
            await _ctaDal.CreateAsync(mapped);
        }

        public async Task DeleteAsync(string id)
        {
           await _ctaDal.DeleteAsync(id);
        }

        public async Task<List<ResultCtaDto>> GetAllAsync()
        {
            var values=await _ctaDal.GetAllAsync();
            return  _mapper.Map<List<ResultCtaDto>>(values);
        }

        public async Task<UpdateCtaDto> GetByIdAsync(string id)
        {
            var values=await _ctaDal.GetByIdAsync(id);
            return _mapper.Map<UpdateCtaDto>(values);
        }

        public async Task UpdateAsync(UpdateCtaDto update)
        {
            var mapped = _mapper.Map<Cta>(update);
            await _ctaDal.UpdateAsync(mapped);
        }
    }
}
