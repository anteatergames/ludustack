using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum SoundStyle
    {
        [Display(Name = "Chiptune")]
        Chiptune = 1,

        [Display(Name = "SNES Era")]
        SnesEra = 2,

        [Display(Name = "Orchestral")]
        Orchestral = 3
    }
}