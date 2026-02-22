using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.PartnerDtos
{
    public class CreatePartnerDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
    }
}
