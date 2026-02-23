using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.UserDtos
{
    public class UserFavoriteDto
    {
        public string Id { get; set; }
        public string DestinationId { get; set; }
        public string CityCountry { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
