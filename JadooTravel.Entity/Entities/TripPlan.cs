using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class TripPlan: BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
    }
}
