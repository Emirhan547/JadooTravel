using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ChatDtos
{
    public class ChatMessageResponseDto
    {
        public string Id { get; set; }
        public string UserMessage { get; set; }
        public string BotResponse { get; set; }
        public bool IsAutomatic { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RespondedDate { get; set; }
    }
}
