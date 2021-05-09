using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum Dimensions
    {
        [Display(Name = "2D")]
        TwoDee = 1,

        [Display(Name = "3D")]
        ThreeDee = 2,
    }
}