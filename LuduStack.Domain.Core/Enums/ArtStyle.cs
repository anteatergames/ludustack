using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum ArtStyle
    {
        [UiInfo(Display = "Pixel Art")]
        [Display(Name = "Pixel Art")]
        PixelArt = 1,

        [UiInfo(Display = "Vector Art")]
        [Display(Name = "Vector Art")]
        Vector = 2,

        [UiInfo(Display = "Artistic")]
        [Display(Name = "Artistic")]
        Artistic = 3,

        [UiInfo(Display = "Realistic")]
        [Display(Name = "Realistic")]
        Realistic = 4
    }
}