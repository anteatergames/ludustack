using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class GameIdea : Entity
    {
        public SupportedLanguage Language { get; set; }

        public GameIdeaElementType Type { get; set; }

        public string Description { get; set; }
    }
}