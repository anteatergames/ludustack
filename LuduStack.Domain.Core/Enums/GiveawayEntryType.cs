using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GiveawayEntryType
    {
        [UiInfo(Display = "Entered", AllowMultiple = false)]
        LoginOrEmail = 1,

        [UiInfo(Display = "Email confirmed", AllowMultiple = false)]
        EmailConfirmed = 2,

        [UiInfo(Display = "Referral code", AllowMultiple = false)]
        ReferralCode = 3,

        [UiInfo(Display = "Daily Entry", AllowMultiple = true )]
        Daily = 4
    }
}