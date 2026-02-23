using JadooTravel.Dto.Dtos.AnalyticsDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IAnalyticsService
    {
        Task TrackPageViewAsync(string pageUrl, string pageName, string userId, string sessionId);
        Task TrackConversionAsync(string userId, string conversionType, string conversionValue, decimal? amount);

        // İstatistikler
        Task<AnalyticsOverviewDto> GetOverviewAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<DailyStatsDto>> GetDailyStatsAsync(DateTime startDate, DateTime endDate);
        Task<MonthlyStatsDto> GetMonthlyStatsAsync(int month, int year);

        // Destinasyon Analytics
        Task<List<DestinationAnalyticsDto>> GetDestinationAnalyticsAsync();
        Task<DestinationAnalyticsDto> GetDestinationAnalyticsByIdAsync(string destinationId);

        // Kullanıcı Davranışı
        Task<UserBehaviorDto> GetUserBehaviorAsync(string userId);
        Task<List<UserBehaviorDto>> GetTopUsersAsync(int limit = 10);

        // Trafik Kaynakları
        Task<List<TrafficSourceDto>> GetTrafficSourcesAsync();
    }
}
