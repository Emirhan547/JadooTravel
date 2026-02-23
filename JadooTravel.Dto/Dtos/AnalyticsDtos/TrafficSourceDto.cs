using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class TrafficSourceDto
    {
        public string Source { get; set; } // "direct", "search", "social", "referral"
        public int Visits { get; set; }
        public int Conversions { get; set; }
        public decimal Revenue { get; set; }
    }
}
