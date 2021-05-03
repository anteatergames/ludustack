using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.ValueObjects
{
    public class MediaListItemVo
    {
        public SupportedLanguage? Language { get; set; }

        public MediaType? Type { get; set; }

        public string Image { get; set; }

        public string ImageLquip { get; set; }

        public string ImageResponsive { get; set; }
    }
}