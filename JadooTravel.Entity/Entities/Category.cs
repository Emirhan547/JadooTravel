using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class Category:BaseEntity
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public bool Status { get; set; }
    }
}
