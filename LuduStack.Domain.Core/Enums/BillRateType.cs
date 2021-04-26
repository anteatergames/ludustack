using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum BillRateType
    {
        [Display(Name = "Visual")]
        Visual = 1,

        [Display(Name = "Audio")]
        Audio = 2,

        [Display(Name = "Code")]
        Code = 3,

        [Display(Name = "Text")]
        Text = 4
    }
}