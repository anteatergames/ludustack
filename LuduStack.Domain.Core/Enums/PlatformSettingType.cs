using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum PlatformSettingType
    {
        [UiInfo(Display = "Boolean")]
        [Display(Name = "Boolean")]
        Boolean = 1,

        [UiInfo(Display = "Integer")]
        [Display(Name = "Integer")]
        Integer = 2,

        [UiInfo(Display = "Text")]
        [Display(Name = "Text")]
        Text = 3,
    }
}