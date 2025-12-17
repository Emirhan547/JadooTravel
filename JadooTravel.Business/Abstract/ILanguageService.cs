using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface ILanguageService
    {
        string GetString(string key);
        void SetLanguage(string culture);
        string GetCurrentLanguage();
    }
}
