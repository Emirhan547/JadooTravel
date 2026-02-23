using JadooTravel.Entity.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ChatDtos
{
    public class ChatHistoryDto
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public ChatCategory Category { get; set; }
        public bool IsAutomatic { get; set; }
        public int? Satisfaction { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
