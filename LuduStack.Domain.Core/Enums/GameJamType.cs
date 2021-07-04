using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamType
    {
        [UiInfo(Display = "Ranked")]
        [Display(Name = "Ranked")]
        Ranked = 1,

        [UiInfo(Display = "Not Ranked")]
        [Display(Name = "Not Ranked")]
        NotRanked = 2,
    }
}