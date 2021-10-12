using LuduStack.Domain.Core.Enums;
using System.Collections.Generic;

namespace LuduStack.Web.Models
{
    public class ExcelColumnViewModel
    {
        public int Column { get; set; }
        public LocalizationLanguage Language { get; set; }
    }

    public static class ExcelColumnViewModelExtensions
    {
        public static IEnumerable<KeyValuePair<int, LocalizationLanguage>> ToKeyValuePairs(this IEnumerable<ExcelColumnViewModel> list)
        {
            List<KeyValuePair<int, LocalizationLanguage>> kvList = new List<KeyValuePair<int, LocalizationLanguage>>();

            foreach (ExcelColumnViewModel item in list)
            {
                if (item.Language != 0)
                {
                    kvList.Add(new KeyValuePair<int, LocalizationLanguage>(item.Column, item.Language));
                }
            }

            return kvList;
        }
    }
}