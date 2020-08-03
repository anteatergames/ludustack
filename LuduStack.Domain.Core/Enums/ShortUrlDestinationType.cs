using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum ShortUrlDestinationType
    {
        [Display(Name = "Undefined")]
        Undefined = 0,

        [Display(Name = "Giveaway")]
        Giveaway = 1
    }
}