using JadooTravel.Dto.Dtos.ChatDtos;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IChatbotService
    {
        Task<ChatMessageResponseDto> SendMessageAsync(SendMessageDto sendMessageDto, string userId);
        Task<List<ChatHistoryDto>> GetChatHistoryAsync(string userId);
        Task RateChatAsync(RateChatDto rateChatDto);

        // FAQ
        Task<List<FAQDto>> GetFAQsAsync(string category = null);
        Task<FAQDto> CreateFAQAsync(CreateFAQDto createFAQDto);
        Task UpdateFAQAsync(string faqId, CreateFAQDto updateFAQDto);
        Task DeleteFAQAsync(string faqId);
        Task MarkFAQHelpfulAsync(string faqId, bool helpful);

        // Chatbot Templates
        Task<List<ChatbotTemplate>> GetTemplatesAsync();
        Task CreateTemplateAsync(CreateChatbotTemplateDto createTemplateDto);
        Task DeleteTemplateAsync(string templateId);

        // İstatistikler
        Task<ChatStatsDto> GetChatStatsAsync();

        // Admin - Operatör Cevapları
        Task RespondAsOperatorAsync(string userId, OperatorResponseDto operatorResponseDto);
        Task<List<ChatMessageResponseDto>> GetUnansweredMessagesAsync();
    }
}
