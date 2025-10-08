using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.DestinationDtos
{
    public class GetDestinationByIdDto
    {
        public ObjectId Id { get; set; }
        public string CityCountry { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string DayNight { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
    }
}
