using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameIdeaElementType
    {
        [UiInfo(Display = "Genre")]
        [Display(Name = "Genre")]
        Genre = 1,

        [UiInfo(Display = "Action")]
        [Display(Name = "Action")]
        Action = 2,

        [UiInfo(Display = "Thing")]
        [Display(Name = "Thing")]
        Thing = 3,

        [UiInfo(Display = "Goal")]
        [Display(Name = "Goal")]
        Goal = 4,

        [UiInfo(Display = "Rule")]
        [Display(Name = "Rule")]
        Rule = 5,

        [UiInfo(Display = "Preffix")]
        [Display(Name = "Preffix")]
        Preffix = 6,

        [UiInfo(Display = "Suffix")]
        [Display(Name = "Suffix")]
        Suffix = 7
    }
}