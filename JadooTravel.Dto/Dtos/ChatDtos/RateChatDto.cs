using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ChatDtos
{
    public class RateChatDto
    {
        public string ChatMessageId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
    }
}
