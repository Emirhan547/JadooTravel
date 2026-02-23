using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.AnalyticsDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class AnalyticsService : IAnalyticsService
    {
     
        private readonly IMongoDatabase _mongoDatabase;

        public AnalyticsService(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task TrackPageViewAsync(string pageUrl, string pageName, string userId, string sessionId)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");

                var pageView = new PageViewTrack
                {
                    PageUrl = pageUrl,
                    PageName = pageName,
                    UserId = userId,
                    SessionId = sessionId,
                    ViewedAt = DateTime.UtcNow
                };

                await collection.InsertOneAsync(pageView);
            }
            catch { }
        }

        public async Task TrackConversionAsync(string userId, string conversionType, string conversionValue, decimal? amount)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");

                var conversion = new ConversionTrack
                {
                    UserId = userId,
                    ConversionType = conversionType,
                    ConversionValue = conversionValue,
                    Amount = amount ?? 0,
                    ConvertedAt = DateTime.UtcNow
                };

                await collection.InsertOneAsync(conversion);
            }
            catch { }
        }

        public async Task<AnalyticsOverviewDto> GetOverviewAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.UtcNow.AddDays(-30);
                endDate ??= DateTime.UtcNow;

                var pageViewCollection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");
                var conversionCollection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");

                var pageViewFilter = Builders<PageViewTrack>.Filter.And(
                    Builders<PageViewTrack>.Filter.Gte(x => x.ViewedAt, startDate),
                    Builders<PageViewTrack>.Filter.Lte(x => x.ViewedAt, endDate)
                );

                var allPageViews = await pageViewCollection.Find(pageViewFilter).ToListAsync();
                var uniqueVisitors = allPageViews.Select(x => x.SessionId).Distinct().Count();

                var conversionFilter = Builders<ConversionTrack>.Filter.And(
                    Builders<ConversionTrack>.Filter.Gte(x => x.ConvertedAt, startDate),
                    Builders<ConversionTrack>.Filter.Lte(x => x.ConvertedAt, endDate)
                );

                var conversions = await conversionCollection.Find(conversionFilter).ToListAsync();
                var bookings = conversions.Count(x => x.ConversionType == "booking");
                var revenue = conversions.Where(x => x.ConversionType == "booking").Sum(x => x.Amount);

                var conversionRate = allPageViews.Count > 0
                    ? (conversions.Count / (double)allPageViews.Count) * 100
                    : 0;

                var last30Days = GetDailyStatsAsync(startDate.Value, endDate.Value).Result;

                return new AnalyticsOverviewDto
                {
                    TotalPageViews = allPageViews.Count,
                    UniqueVisitors = uniqueVisitors,
                    TotalBookings = bookings,
                    TotalRevenue = revenue,
                    ConversionRate = Math.Round(conversionRate, 2),
                    BounceRate = CalculateBounceRate(allPageViews),
                    AverageSessionDuration = CalculateAverageSessionDuration(allPageViews),
                    Last30DaysStats = last30Days
                };
            }
            catch
            {
                return new AnalyticsOverviewDto();
            }
        }

        public async Task<List<DailyStatsDto>> GetDailyStatsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var pageViewCollection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");
                var conversionCollection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");

                var allPageViews = await pageViewCollection.Find(_ => true).ToListAsync();
                var allConversions = await conversionCollection.Find(_ => true).ToListAsync();

                var dailyStats = new List<DailyStatsDto>();

                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var pageViewsThatDay = allPageViews
                        .Where(x => x.ViewedAt.Date == date.Date)
                        .ToList();

                    var conversionsThatDay = allConversions
                        .Where(x => x.ConvertedAt.Date == date.Date)
                        .ToList();

                    var bookingsThatDay = conversionsThatDay.Count(x => x.ConversionType == "booking");
                    var revenueThatDay = conversionsThatDay
                        .Where(x => x.ConversionType == "booking")
                        .Sum(x => x.Amount);

                    dailyStats.Add(new DailyStatsDto
                    {
                        Date = date,
                        PageViews = pageViewsThatDay.Count,
                        UniqueVisitors = pageViewsThatDay.Select(x => x.SessionId).Distinct().Count(),
                        Bookings = bookingsThatDay,
                        Revenue = revenueThatDay,
                        AverageSessionDuration = pageViewsThatDay.Count > 0
                            ? pageViewsThatDay.Average(x => x.DurationSeconds)
                            : 0,
                        ConversionRate = pageViewsThatDay.Count > 0
                            ? (conversionsThatDay.Count / (double)pageViewsThatDay.Count) * 100
                            : 0
                    });
                }

                return dailyStats;
            }
            catch
            {
                return new List<DailyStatsDto>();
            }
        }

        public async Task<MonthlyStatsDto> GetMonthlyStatsAsync(int month, int year)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var pageViewCollection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");
                var conversionCollection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");

                var pageViewFilter = Builders<PageViewTrack>.Filter.And(
                    Builders<PageViewTrack>.Filter.Gte(x => x.ViewedAt, startDate),
                    Builders<PageViewTrack>.Filter.Lte(x => x.ViewedAt, endDate)
                );

                var conversionFilter = Builders<ConversionTrack>.Filter.And(
                    Builders<ConversionTrack>.Filter.Gte(x => x.ConvertedAt, startDate),
                    Builders<ConversionTrack>.Filter.Lte(x => x.ConvertedAt, endDate)
                );

                var pageViews = await pageViewCollection.Find(pageViewFilter).ToListAsync();
                var conversions = await conversionCollection.Find(conversionFilter).ToListAsync();

                var dailyStats = GetDailyStatsAsync(startDate, endDate).Result;

                return new MonthlyStatsDto
                {
                    Month = month,
                    Year = year,
                    TotalPageViews = pageViews.Count,
                    UniqueUsers = pageViews.Select(x => x.SessionId).Distinct().Count(),
                    TotalBookings = conversions.Count(x => x.ConversionType == "booking"),
                    TotalRevenue = conversions.Where(x => x.ConversionType == "booking").Sum(x => x.Amount),
                    AverageConversionRate = dailyStats.Average(x => x.ConversionRate)
                };
            }
            catch
            {
                return new MonthlyStatsDto { Month = month, Year = year };
            }
        }

        public async Task<List<DestinationAnalyticsDto>> GetDestinationAnalyticsAsync()
        {
            try
            {
                var destinationCollection = _mongoDatabase.GetCollection<Destination>("Destinations");
                var pageViewCollection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");
                var conversionCollection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var destinations = await destinationCollection.Find(_ => true).ToListAsync();
                var pageViews = await pageViewCollection.Find(_ => true).ToListAsync();
                var conversions = await conversionCollection.Find(_ => true).ToListAsync();
                var reviews = await reviewCollection.Find(x => x.IsApproved).ToListAsync();

                var result = new List<DestinationAnalyticsDto>();

                foreach (var dest in destinations)
                {
                    var destPageViews = pageViews.Count(x => x.PageUrl.Contains(dest.Id));
                    var destConversions = conversions.Count(x =>
                        x.ConversionType == "booking" && x.ConversionValue == dest.Id);
                    var destRevenue = conversions
                        .Where(x => x.ConversionType == "booking" && x.ConversionValue == dest.Id)
                        .Sum(x => x.Amount);
                    var destReviews = reviews.Where(x => x.DestinationId == dest.Id).ToList();

                    result.Add(new DestinationAnalyticsDto
                    {
                        DestinationId = dest.Id,
                        CityCountry = dest.CityCountry,
                        PageViews = destPageViews,
                        Bookings = destConversions,
                        Revenue = destRevenue,
                        ConversionRate = destPageViews > 0
                            ? (destConversions / (double)destPageViews) * 100
                            : 0,
                        ReviewCount = destReviews.Count,
                        AverageRating = destReviews.Count > 0
                            ? (int)destReviews.Average(x => x.Rating)
                            : 0
                    });
                }

                return result.OrderByDescending(x => x.Revenue).ToList();
            }
            catch
            {
                return new List<DestinationAnalyticsDto>();
            }
        }

        public async Task<DestinationAnalyticsDto> GetDestinationAnalyticsByIdAsync(string destinationId)
        {
            try
            {
                var analytics = await GetDestinationAnalyticsAsync();
                return analytics.FirstOrDefault(x => x.DestinationId == destinationId);
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserBehaviorDto> GetUserBehaviorAsync(string userId)
        {
            try
            {
                var pageViewCollection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");
                var conversionCollection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");

                var pageViews = await pageViewCollection.Find(x => x.UserId == userId).ToListAsync();
                var conversions = await conversionCollection.Find(x => x.UserId == userId).ToListAsync();

                return new UserBehaviorDto
                {
                    UserId = userId,
                    FirstVisit = pageViews.Count > 0 ? pageViews.Min(x => x.ViewedAt) : DateTime.UtcNow,
                    LastVisit = pageViews.Count > 0 ? pageViews.Max(x => x.ViewedAt) : DateTime.UtcNow,
                    TotalVisits = pageViews.Select(x => x.SessionId).Distinct().Count(),
                    TotalPageViews = pageViews.Count,
                    AverageSessionDuration = pageViews.Count > 0
                        ? pageViews.Average(x => x.DurationSeconds)
                        : 0,
                    Bookings = conversions.Count(x => x.ConversionType == "booking"),
                    TotalSpent = conversions.Where(x => x.ConversionType == "booking").Sum(x => x.Amount)
                };
            }
            catch
            {
                return new UserBehaviorDto { UserId = userId };
            }
        }

        public async Task<List<UserBehaviorDto>> GetTopUsersAsync(int limit = 10)
        {
            try
            {
                var conversionCollection = _mongoDatabase.GetCollection<ConversionTrack>("ConversionTracks");

                var conversions = await conversionCollection.Find(_ => true).ToListAsync();
                var topUserIds = conversions
                    .GroupBy(x => x.UserId)
                    .OrderByDescending(x => x.Sum(y => y.Amount))
                    .Take(limit)
                    .Select(x => x.Key)
                    .ToList();

                var result = new List<UserBehaviorDto>();
                foreach (var userId in topUserIds)
                {
                    var behavior = await GetUserBehaviorAsync(userId);
                    result.Add(behavior);
                }

                return result;
            }
            catch
            {
                return new List<UserBehaviorDto>();
            }
        }

        public async Task<List<TrafficSourceDto>> GetTrafficSourcesAsync()
        {
            try
            {
                var pageViewCollection = _mongoDatabase.GetCollection<PageViewTrack>("PageViewTracks");
                var pageViews = await pageViewCollection.Find(_ => true).ToListAsync();

                var sources = new Dictionary<string, int>();

                foreach (var pv in pageViews)
                {
                    var source = DetermineTrafficSource(pv.ReferrerUrl);
                    if (sources.ContainsKey(source))
                        sources[source]++;
                    else
                        sources[source] = 1;
                }

                return sources.Select(x => new TrafficSourceDto
                {
                    Source = x.Key,
                    Visits = x.Value
                }).OrderByDescending(x => x.Visits).ToList();
            }
            catch
            {
                return new List<TrafficSourceDto>();
            }
        }

        private string DetermineTrafficSource(string referrer)
        {
            if (string.IsNullOrEmpty(referrer))
                return "direct";

            if (referrer.Contains("google"))
                return "search";
            if (referrer.Contains("facebook") || referrer.Contains("twitter") || referrer.Contains("instagram"))
                return "social";

            return "referral";
        }

        private double CalculateBounceRate(List<PageViewTrack> pageViews)
        {
            if (pageViews.Count == 0)
                return 0;

            var sessionsWithOnePage = pageViews
                .GroupBy(x => x.SessionId)
                .Count(x => x.Count() == 1);

            return (sessionsWithOnePage / (double)pageViews.GroupBy(x => x.SessionId).Count()) * 100;
        }

        private double CalculateAverageSessionDuration(List<PageViewTrack> pageViews)
        {
            if (pageViews.Count == 0)
                return 0;

            return pageViews.Average(x => x.DurationSeconds);
        }
    }
}