using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum ProductOrigin
    {
        [UiInfo(Display = "External Store")]
        [Display(Name = "External Store")]
        ExternalStore = 1
    }
}