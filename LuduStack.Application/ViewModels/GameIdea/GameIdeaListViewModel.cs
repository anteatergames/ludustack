using LuduStack.Domain.Core.Enums;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.GameIdea
{
    public class GameIdeaListViewModel
    {
        public SupportedLanguage Language { get; set; }

        public Dictionary<GameIdeaElementType, List<GameIdeaViewModel>> Elements { get; set; }
    }
}