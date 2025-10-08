using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.TestimonialDtos
{
    public class UpdateTestimonialDto
    {
        public ObjectId Id { get; set; }
        public string NameSurname { get; set; }
        public string Comment { get; set; }
        public string ImageUrl { get; set; }
        public string JobTitle { get; set; }
    }
}
