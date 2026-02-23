using JadooTravel.Business.Abstract;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace JadooTravel.UI.Middleware
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAnalyticsService _analyticsService;
        private readonly string _sessionIdCookieName = "SessionId";

        public AnalyticsMiddleware(RequestDelegate next, IAnalyticsService analyticsService)
        {
            _next = next;
            _analyticsService = analyticsService;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
        {
            // Session ID al veya oluştur
            if (!context.Request.Cookies.ContainsKey(_sessionIdCookieName))
            {
                context.Response.Cookies.Append(
                    _sessionIdCookieName,
                    Guid.NewGuid().ToString(),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
                );
            }

            var sessionId = context.Request.Cookies[_sessionIdCookieName];
            var userId = userManager.GetUserAsync(context.User)?.Result?.Id;
            var pageUrl = context.Request.Path.Value;

            // Page View'i takip et (sadece GET ve HTML için)
            if (context.Request.Method == "GET" && IsBrowserRequest(context))
            {
                _ = _analyticsService.TrackPageViewAsync(
                    pageUrl,
                    GetPageName(pageUrl),
                    userId,
                    sessionId
                );
            }

            await _next(context);
        }

        private bool IsBrowserRequest(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            return !userAgent.Contains("bot") && !userAgent.Contains("crawler");
        }

        private string GetPageName(string url)
        {
            if (url.Contains("/destination/"))
                return "Destination Detail";
            if (url.Contains("/profile"))
                return "User Profile";
            if (url == "/")
                return "Home Page";
            return url;
        }
    }

}