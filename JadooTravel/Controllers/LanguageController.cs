// JadooTravel.UI/Controllers/LanguageController.cs

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Controllers
{
    public class LanguageController : Controller
    {
        /// <summary>
        /// Kullanıcının dil tercihini değiştirir ve cookie'ye kaydeder
        /// </summary>
        /// <param name="culture">Seçilen dil kodu (örn: tr-TR, en-US)</param>
        /// <param name="returnUrl">Geri dönülecek sayfa URL'i</param>
        /// <returns>Önceki sayfaya yönlendirme</returns>
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            // Dil seçimini cookie'ye kaydet
            Response.Cookies.Append(
                "UserLanguage", // Cookie adı
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), // Cookie değeri
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1), // 1 yıl geçerli
                    IsEssential = true, // GDPR için gerekli
                    Path = "/", // Tüm sayfalarda geçerli
                    HttpOnly = false, // JavaScript'ten erişilebilir
                    Secure = false // HTTPS zorunlu değil (production'da true yapın)
                }
            );

            // Eğer returnUrl boşsa anasayfaya yönlendir
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}