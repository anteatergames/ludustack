using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamPhase
    {
        [UiInfo(Display = "Warmup")]
        [Display(Name = "Warmup")]
        Warmup = 1,

        [UiInfo(Display = "Submission")]
        [Display(Name = "Submission")]
        Submission = 2,

        [UiInfo(Display = "Voting")]
        [Display(Name = "Voting")]
        Voting = 3,

        [UiInfo(Display = "Results")]
        [Display(Name = "Results")]
        Results = 4,

        [UiInfo(Display = "Finished")]
        [Display(Name = "Finished")]
        Finished = 5,
    }
}