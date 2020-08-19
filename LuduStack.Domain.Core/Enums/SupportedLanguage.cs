using LuduStack.Domain.Core.Attributes;

namespace LuduStack.Domain.Core.Enums
{
    public enum SupportedLanguage
    {
        [UiInfo(Locale = "en-US")]
        English = 1,

        [UiInfo(Locale = "pt-BR")]
        Portuguese = 2,

        [UiInfo(Locale = "ru-RU")]
        Russian = 3,

        [UiInfo(Locale = "es")]
        Spanish = 4,

        [UiInfo(Locale = "bs")]
        Bosnian = 5,

        [UiInfo(Locale = "sr")]
        Serbian = 6,

        [UiInfo(Locale = "hr")]
        Croatian = 7,

        [UiInfo(Locale = "de")]
        German = 8
    }
}