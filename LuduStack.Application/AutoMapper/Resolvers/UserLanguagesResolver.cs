using AutoMapper;
using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.AutoMapper.Resolvers
{
    public class UserLanguagesToDomainResolver : IValueResolver<UserPreferencesViewModel, UserPreferences, string>
    {
        public string Resolve(UserPreferencesViewModel source, UserPreferences destination, string destMember, ResolutionContext context)
        {
            string result = string.Empty;

            if (source.Languages == null || !source.Languages.Any())
            {
                return result;
            }

            result = string.Join('|', source.Languages);

            return result;
        }
    }

    public class UserLanguagesFromDomainResolver : IValueResolver<UserPreferences, UserPreferencesViewModel, List<SupportedLanguage>>
    {
        public List<SupportedLanguage> Resolve(UserPreferences source, UserPreferencesViewModel destination, List<SupportedLanguage> destMember, ResolutionContext context)
        {
            string[] languages = (source.ContentLanguages ?? string.Empty).Split(new[] { '|' });

            IEnumerable<SupportedLanguage> languagesConverted = languages.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), x));

            return languagesConverted.ToList();
        }
    }
}