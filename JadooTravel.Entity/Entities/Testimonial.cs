using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class Testimonial: BaseEntity
    {
        public string NameSurname { get; set; }
        public string Comment { get; set; }
        public string ImageUrl { get; set; }
        public string JobTitle { get; set; }
    }
}
