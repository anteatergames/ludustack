using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum PlatformSettingGroup
    {

        [UiInfo(Display = "Feature")]
        [Display(Name = "Feature")]
        Feature = 1,

        [UiInfo(Display = "General")]
        [Display(Name = "General")]
        General = 2,

        [UiInfo(Display = "Home")]
        [Display(Name = "Home")]
        Home = 3,

        [UiInfo(Display = "Feed")]
        [Display(Name = "Feed")]
        Feed = 4,

        [UiInfo(Display = "Forum")]
        [Display(Name = "Forum")]
        Forum = 5,

        [UiInfo(Display = "Store")]
        [Display(Name = "Store")]
        Store = 6,
    }
}