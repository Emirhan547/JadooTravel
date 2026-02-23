using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ChatDtos
{
    public class ChatStatsDto
    {
        public int TotalMessages { get; set; }
        public int AutomaticResponses { get; set; }
        public int OperatorResponses { get; set; }
        public double AverageSatisfaction { get; set; }
        public int MessagesThisMonth { get; set; }
        public Dictionary<string, int> MessagesByCategory { get; set; }
    }
}
