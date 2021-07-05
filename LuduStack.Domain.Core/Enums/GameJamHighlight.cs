using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamHighlight
    {
        [Display(Name = "Not Informed")]
        [UiInfo(Display = "Not Informed", Class = "times")]
        NotInformed = 0,

        [Display(Name = "Public Submissions")]
        [UiInfo(Display = "Public Submissions", Class = "handshake", Description = ("All submissions to this Game Jam are public and they can be seen during the submission phase."))]
        SubmissionsPublic,

        [Display(Name = "Secret Submissions")]
        [UiInfo(Display = "Secret Submissions", Class = "handshake-slash", Description = ("All submissions to this Game Jam would not be visible during the submission phase."))]
        SubmissionsSecret,

        [Display(Name = "Public Results")]
        [UiInfo(Display = "Public Results", Class = "eye", Description = ("The voting results can bee seen in realtime as they happen."))]
        ResultsPublic,

        [Display(Name = "Secret Results")]
        [UiInfo(Display = "Secret Results", Class = "eye-slash", Description = ("The voting results will only be visible after the voting phase."))]
        ResultsSecret,

        [Display(Name = "Unlisted")]
        [UiInfo(Display = "Unlisted", Class = "store-alt-slash", Description = ("This Game Jam is not listed on the LuduStack platform. Only people with the direct URL can see it."))]
        Unlisted,

        [Display(Name = "Secret Theme")]
        [UiInfo(Display = "Secret Theme", Class = "user-secret", Description = ("The Main Theme will be hidden during the warmup phase."))]
        MainThemeSecret,

        [Display(Name = "Late Join Forbidden")]
        [UiInfo(Display = "Late Join Forbidden", Class = "clock", Description = ("Users can only join this Game Jam during the warmup phase."))]
        LateJoinForbidden,
    }
}