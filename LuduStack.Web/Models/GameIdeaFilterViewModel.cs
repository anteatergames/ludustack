using LuduStack.Domain.Core.Enums;

namespace LuduStack.Web.Models
{
    public class GameIdeaFilterViewModel
    {
        public SupportedLanguage Language { get; set; }

        public GameIdeaElementType Type { get; set; }
    }
}
