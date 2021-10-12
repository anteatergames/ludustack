using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.Localization
{
    public class LocalizationEntryViewModel : UserGeneratedBaseViewModel
    {
        public Guid TermId { get; set; }

        public LocalizationLanguage Language { get; set; }

        public string Value { get; set; }

        public bool? Accepted { get; set; }
    }
}