using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum ArtStyle
    {
        [Display(Name = "Pixel Art")]
        PixelArt = 1,

        [Display(Name = "Vector Art")]
        Vector = 2,

        [Display(Name = "Artistic")]
        Artistic = 3,

        [Display(Name = "Realistic")]
        Realistic = 4
    }
}