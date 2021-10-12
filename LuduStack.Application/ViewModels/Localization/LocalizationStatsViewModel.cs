using LuduStack.Domain.Core.Enums;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Localization
{
    public class TranslationStatsViewModel : LocalizationViewModel
    {
        public List<ContributorViewModel> Contributors { get; set; }
        public List<TranslationStatsLanguageViewModel> Languages { get; set; }

        public TranslationStatsViewModel()
        {
            Contributors = new List<ContributorViewModel>();
            Languages = new List<TranslationStatsLanguageViewModel>();
        }
    }

    public class TranslationStatsLanguageViewModel
    {
        public LocalizationLanguage Language { get; set; }

        public int EntryCount { get; set; }

        public double Percentage { get; set; }

        public string PercentageText
        {
            get
            {
                string text = Percentage.ToString("N1");

                if (Percentage == 0 || Percentage == 100 || text.Substring(text.Length - 1).Equals("0"))
                {
                    return Percentage.ToString("N0");
                }
                else
                {
                    return text;
                }
            }
        }
    }
}