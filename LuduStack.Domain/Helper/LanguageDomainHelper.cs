using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Domain.Helper
{
    public static class LanguageDomainHelper
    {
        public static IEnumerable<SupportedLanguage> FormatList(string contentLanguages)
        {
            string[] languages = (contentLanguages ?? string.Empty).Split(new[] { '|' });

            IEnumerable<SupportedLanguage> languagesConverted = languages.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), x));

            return languagesConverted.ToList();
        }
    }
}