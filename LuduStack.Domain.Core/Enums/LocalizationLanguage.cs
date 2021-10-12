using LuduStack.Domain.Core.Attributes;

namespace LuduStack.Domain.Core.Enums
{
    public enum LocalizationLanguage
    {
        [UiInfo(Locale = "en-US")]
        English = 1,

        [UiInfo(Locale = "pt-BR")]
        Portuguese = 2,

        [UiInfo(Locale = "ru-RU")]
        Russian = 3,

        [UiInfo(Locale = "es")]
        Spanish = 4,

        [UiInfo(Locale = "de")]
        German = 5,

        [UiInfo(Locale = "fr")]
        French = 6
    }
}