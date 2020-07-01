using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GiveawayStatus
    {
        [Display(Name = "Draft")]
        Draft = 1,

        [Display(Name = "Pending Start")]
        PendingStart = 2,

        [Display(Name = "Open for Entries")]
        OpenForEntries = 3,

        [Display(Name = "Picking Winners")]
        PickingWinners = 4,

        [Display(Name = "Ended")]
        Ended = 5
    }
}