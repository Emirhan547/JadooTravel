using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ChatDtos
{
    public class CreateFAQDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }
        public int Priority { get; set; }
    }
}
