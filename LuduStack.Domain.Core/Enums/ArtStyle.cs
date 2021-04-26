using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum ArtStyle
    {
        [UiInfo(Display = "Pixel")]
        [Display(Name = "Pixel")]
        Pixel = 1,

        [UiInfo(Display = "Flat")]
        [Display(Name = "Flat")]
        Flat = 2,

        [UiInfo(Display = "Cartoon")]
        [Display(Name = "Cartoon")]
        Cartoon = 3,

        [UiInfo(Display = "Artistic")]
        [Display(Name = "Artistic")]
        Artistic = 4,

        [UiInfo(Display = "Realistic")]
        [Display(Name = "Realistic")]
        Realistic = 5
    }
}