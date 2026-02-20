using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class Feature: BaseEntity
    {
        public string Title { get; set; }
        public string MainTitle { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }

    }
}

