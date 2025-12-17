using JadooTravel.Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class LanguageManager : ILanguageService
    {
        // Fix for CS8618: Add constructor to initialize _localizer and _httpContextAccessor.
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageManager(
             IStringLocalizer<SharedResource> localizer,
             IHttpContextAccessor httpContextAccessor)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetString(string key)
        {
            return _localizer[key];
        }

        public void SetLanguage(string culture)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                "UserLanguage",
                culture,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        public string GetCurrentLanguage()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies["UserLanguage"] ?? "tr-TR";
        }
    }
}
