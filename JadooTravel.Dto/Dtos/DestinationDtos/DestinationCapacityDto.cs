using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.DestinationDtos
{
    public class DestinationCapacityDto
    {
        public string CityCountry { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
    }
}
