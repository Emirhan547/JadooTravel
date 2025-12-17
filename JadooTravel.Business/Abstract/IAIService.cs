using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IAIService
    {
        Task<string> GetCityRecommendationsAsync(string cityCountry);
    }
}
