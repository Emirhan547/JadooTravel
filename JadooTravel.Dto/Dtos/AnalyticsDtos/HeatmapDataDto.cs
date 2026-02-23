using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class HeatmapDataDto
    {
        public string PageUrl { get; set; }
        public List<ClickPoint> ClickPoints { get; set; }
    }
}
