using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum SoundStyle
    {
        [UiInfo(Display = "Chiptune")]
        [Display(Name = "Chiptune")]
        Chiptune = 1,

        [UiInfo(Display = "SNES Era")]
        [Display(Name = "SNES Era")]
        SnesEra = 2,

        [UiInfo(Display = "Orchestral")]
        [Display(Name = "Orchestral")]
        Orchestral = 3
    }
}