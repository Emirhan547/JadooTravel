using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IAIService
    {
        Task<string> GetRecommendationsAsync(string cityCountry, string? targetLanguage, CancellationToken cancellationToken);
        Task<IReadOnlyList<string>> TranslateAsync(string targetLanguage, IReadOnlyList<string> texts, CancellationToken cancellationToken);
    }
}
