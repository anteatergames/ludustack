using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum OrderSituation
    {

        [UiInfo(Display = "Unknown")]
        [Display(Name = "Unknown")]
        Unknown = 1,

        [UiInfo(Display = "Pending")]
        [Display(Name = "Pending")]
        Pending = 2,

        [UiInfo(Display = "Fulfilled")]
        [Display(Name = "Fulfilled")]
        Fulfilled = 3,

        [UiInfo(Display = "Canceled")]
        [Display(Name = "Canceled")]
        Canceled = 4
    }
}