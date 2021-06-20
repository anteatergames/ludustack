using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum PlatformSettingGroup
    {
        [UiInfo(Display = "General")]
        [Display(Name = "General")]
        General = 1,

        [UiInfo(Display = "Home")]
        [Display(Name = "Home")]
        Home = 2,

        [UiInfo(Display = "Feed")]
        [Display(Name = "Feed")]
        Feed = 3,

        [UiInfo(Display = "Forum")]
        [Display(Name = "Forum")]
        Forum = 4,
    }
}