using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.ChatDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.Entity.Entities.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class ChatbotService : IChatbotService
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMapper _mapper;

        public ChatbotService(
            IMongoClient mongoClient,
            IMapper mapper)
        {
            _mongoClient = mongoClient;
            _mongoDatabase = mongoClient.GetDatabase("JadooTravelDb");
            _mapper = mapper;
        }

        public async Task<ChatMessageResponseDto> SendMessageAsync(SendMessageDto sendMessageDto, string userId)
        {
            try
            {
                var chatCollection = _mongoDatabase.GetCollection<ChatMessage>("ChatMessages");
                var userCollection = _mongoDatabase.GetCollection<AppUser>("Users");

                var user = await userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    throw new Exception("Kullanıcı bulunamadı");

                // Otomatik cevap bul
                var response = await FindAutomaticResponseAsync(sendMessageDto.Message, sendMessageDto.Category);

                var chatMessage = new ChatMessage
                {
                    UserId = userId,
                    UserName = user.FullName,
                    UserEmail = user.Email,
                    Message = sendMessageDto.Message,
                    Response = response,
                    Category = sendMessageDto.Category,
                    IsAutomatic = true,
                    CreatedDate = DateTime.UtcNow,
                    RespondedDate = DateTime.UtcNow
                };

                await chatCollection.InsertOneAsync(chatMessage);

                return new ChatMessageResponseDto
                {
                    Id = chatMessage.Id,
                    UserMessage = chatMessage.Message,
                    BotResponse = response,
                    IsAutomatic = true,
                    CreatedDate = chatMessage.CreatedDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Mesaj gönderilirken hata: {ex.Message}");
            }
        }

        private async Task<string> FindAutomaticResponseAsync(string userMessage, ChatCategory category)
        {
            try
            {
                var templateCollection = _mongoDatabase.GetCollection<ChatbotTemplate>("ChatbotTemplates");

                var templates = await templateCollection
                    .Find(x => x.IsActive && x.Category == category)
                    .ToListAsync();

                foreach (var template in templates)
                {
                    if (Regex.IsMatch(userMessage, template.KeywordPattern, RegexOptions.IgnoreCase))
                        return template.Response;
                }

                // Bulunmazsa, genel yanıt döndür
                return "Merhaba! Sizin sorunuz bizi ilgilendiriyor. Detaylı cevap için lütfen biraz sabırla bekleyin.";
            }
            catch
            {
                return "Merhaba! Lütfen sorunuzu daha detaylı açıklayabilir misiniz?";
            }
        }

        public async Task<List<ChatHistoryDto>> GetChatHistoryAsync(string userId)
        {
            try
            {
                var chatCollection = _mongoDatabase.GetCollection<ChatMessage>("ChatMessages");

                var messages = await chatCollection
                    .Find(x => x.UserId == userId)
                    .SortByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return _mapper.Map<List<ChatHistoryDto>>(messages);
            }
            catch (Exception ex)
            {
                throw new Exception($"Chat geçmişi getirilirken hata: {ex.Message}");
            }
        }

        public async Task RateChatAsync(RateChatDto rateChatDto)
        {
            try
            {
                var chatCollection = _mongoDatabase.GetCollection<ChatMessage>("ChatMessages");

                var update = Builders<ChatMessage>.Update
                    .Set(x => x.Satisfaction, rateChatDto.Rating)
                    .Set(x => x.SatisfactionComment, rateChatDto.Comment);

                await chatCollection.UpdateOneAsync(
                    Builders<ChatMessage>.Filter.Eq(x => x.Id, rateChatDto.ChatMessageId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Rating kaydedilirken hata: {ex.Message}");
            }
        }

        public async Task<List<FAQDto>> GetFAQsAsync(string category = null)
        {
            try
            {
                var faqCollection = _mongoDatabase.GetCollection<FAQ>("FAQs");

                var filter = Builders<FAQ>.Filter.Eq(x => x.IsActive, true);
                if (!string.IsNullOrEmpty(category))
                    filter = Builders<FAQ>.Filter.And(filter,
                        Builders<FAQ>.Filter.Eq(x => x.Category, category));

                var faqs = await faqCollection
                    .Find(filter)
                    .SortByDescending(x => x.Priority)
                    .ToListAsync();

                return _mapper.Map<List<FAQDto>>(faqs);
            }
            catch (Exception ex)
            {
                throw new Exception($"SSS'ler getirilirken hata: {ex.Message}");
            }
        }

        public async Task<FAQDto> CreateFAQAsync(CreateFAQDto createFAQDto)
        {
            try
            {
                var faqCollection = _mongoDatabase.GetCollection<FAQ>("FAQs");

                var faq = new FAQ
                {
                    Question = createFAQDto.Question,
                    Answer = createFAQDto.Answer,
                    Category = createFAQDto.Category,
                    Priority = createFAQDto.Priority,
                    CreatedDate = DateTime.UtcNow
                };

                await faqCollection.InsertOneAsync(faq);
                return _mapper.Map<FAQDto>(faq);
            }
            catch (Exception ex)
            {
                throw new Exception($"SSS oluşturulurken hata: {ex.Message}");
            }
        }

        public async Task UpdateFAQAsync(string faqId, CreateFAQDto updateFAQDto)
        {
            try
            {
                var faqCollection = _mongoDatabase.GetCollection<FAQ>("FAQs");

                var update = Builders<FAQ>.Update
                    .Set(x => x.Question, updateFAQDto.Question)
                    .Set(x => x.Answer, updateFAQDto.Answer)
                    .Set(x => x.Category, updateFAQDto.Category)
                    .Set(x => x.Priority, updateFAQDto.Priority)
                    .Set(x => x.UpdatedDate, DateTime.UtcNow);

                await faqCollection.UpdateOneAsync(
                    Builders<FAQ>.Filter.Eq(x => x.Id, faqId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"SSS güncellenirken hata: {ex.Message}");
            }
        }

        public async Task DeleteFAQAsync(string faqId)
        {
            try
            {
                var faqCollection = _mongoDatabase.GetCollection<FAQ>("FAQs");

                var update = Builders<FAQ>.Update.Set(x => x.IsActive, false);
                await faqCollection.UpdateOneAsync(
                    Builders<FAQ>.Filter.Eq(x => x.Id, faqId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"SSS silinirken hata: {ex.Message}");
            }
        }

        public async Task MarkFAQHelpfulAsync(string faqId, bool helpful)
        {
            try
            {
                var faqCollection = _mongoDatabase.GetCollection<FAQ>("FAQs");

                var update = helpful
                    ? Builders<FAQ>.Update.Inc(x => x.HelpfulCount, 1)
                    : Builders<FAQ>.Update.Inc(x => x.UnhelpfulCount, 1);

                await faqCollection.UpdateOneAsync(
                    Builders<FAQ>.Filter.Eq(x => x.Id, faqId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Helpful işaretlenirken hata: {ex.Message}");
            }
        }

        public async Task<List<ChatbotTemplate>> GetTemplatesAsync()
        {
            try
            {
                var templateCollection = _mongoDatabase.GetCollection<ChatbotTemplate>("ChatbotTemplates");

                var templates = await templateCollection
                    .Find(x => x.IsActive)
                    .ToListAsync();

                return templates;
            }
            catch (Exception ex)
            {
                throw new Exception($"Template'ler getirilirken hata: {ex.Message}");
            }
        }

        public async Task CreateTemplateAsync(CreateChatbotTemplateDto createTemplateDto)
        {
            try
            {
                var templateCollection = _mongoDatabase.GetCollection<ChatbotTemplate>("ChatbotTemplates");

                var template = new ChatbotTemplate
                {
                    KeywordPattern = createTemplateDto.KeywordPattern,
                    Response = createTemplateDto.Response,
                    Category = createTemplateDto.Category,
                    CreatedDate = DateTime.UtcNow
                };

                await templateCollection.InsertOneAsync(template);
            }
            catch (Exception ex)
            {
                throw new Exception($"Template oluşturulurken hata: {ex.Message}");
            }
        }

        public async Task DeleteTemplateAsync(string templateId)
        {
            try
            {
                var templateCollection = _mongoDatabase.GetCollection<ChatbotTemplate>("ChatbotTemplates");

                var update = Builders<ChatbotTemplate>.Update.Set(x => x.IsActive, false);
                await templateCollection.UpdateOneAsync(
                    Builders<ChatbotTemplate>.Filter.Eq(x => x.Id, templateId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Template silinirken hata: {ex.Message}");
            }
        }

        public async Task<ChatStatsDto> GetChatStatsAsync()
        {
            try
            {
                var chatCollection = _mongoDatabase.GetCollection<ChatMessage>("ChatMessages");

                var allMessages = await chatCollection.Find(_ => true).ToListAsync();
                var thisMonthMessages = allMessages
                    .Where(x => x.CreatedDate.Month == DateTime.UtcNow.Month &&
                               x.CreatedDate.Year == DateTime.UtcNow.Year)
                    .ToList();

                var messagesByCategory = allMessages
                    .GroupBy(x => x.Category.ToString())
                    .ToDictionary(x => x.Key, x => x.Count());

                return new ChatStatsDto
                {
                    TotalMessages = allMessages.Count,
                    AutomaticResponses = allMessages.Count(x => x.IsAutomatic),
                    OperatorResponses = allMessages.Count(x => !x.IsAutomatic),
                    AverageSatisfaction = allMessages.Where(x => x.Satisfaction.HasValue)
                        .Count() > 0
                        ? allMessages.Where(x => x.Satisfaction.HasValue).Average(x => x.Satisfaction.Value)
                        : 0,
                    MessagesThisMonth = thisMonthMessages.Count,
                    MessagesByCategory = messagesByCategory
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"İstatistikler getirilirken hata: {ex.Message}");
            }
        }

        public async Task RespondAsOperatorAsync(string userId, OperatorResponseDto operatorResponseDto)
        {
            try
            {
                var chatCollection = _mongoDatabase.GetCollection<ChatMessage>("ChatMessages");

                var update = Builders<ChatMessage>.Update
                    .Set(x => x.Response, operatorResponseDto.Response)
                    .Set(x => x.IsAutomatic, false)
                    .Set(x => x.RespondedDate, DateTime.UtcNow);

                await chatCollection.UpdateOneAsync(
                    Builders<ChatMessage>.Filter.Eq(x => x.Id, operatorResponseDto.ChatMessageId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Operatör yanıtı kaydedilirken hata: {ex.Message}");
            }
        }

        public async Task<List<ChatMessageResponseDto>> GetUnansweredMessagesAsync()
        {
            try
            {
                var chatCollection = _mongoDatabase.GetCollection<ChatMessage>("ChatMessages");

                var unanswered = await chatCollection
                    .Find(x => x.IsAutomatic == false && x.RespondedDate == null)
                    .SortByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return unanswered.Select(x => new ChatMessageResponseDto
                {
                    Id = x.Id,
                    UserMessage = x.Message,
                    BotResponse = x.Response,
                    IsAutomatic = x.IsAutomatic,
                    CreatedDate = x.CreatedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Cevaplanmayan mesajlar getirilirken hata: {ex.Message}");
            }
        }
    }
}
