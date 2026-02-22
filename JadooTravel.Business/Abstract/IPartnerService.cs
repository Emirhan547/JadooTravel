using JadooTravel.Dto.Dtos.PartnerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IPartnerService:IGenericService<ResultPartnerDto,CreatePartnerDto,UpdatePartnerDto>
    {
    }
}
