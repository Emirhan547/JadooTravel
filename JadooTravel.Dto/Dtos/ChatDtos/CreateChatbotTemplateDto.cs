using JadooTravel.Entity.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ChatDtos
{
    public class CreateChatbotTemplateDto
    {
        public string KeywordPattern { get; set; }
        public string Response { get; set; }
        public ChatCategory Category { get; set; }
    }
}
