using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GiveawayEntryType
    {
        [UiInfo(Display = "Entered")]
        LoginOrEmail = 1,

        [UiInfo(Display = "Email confirmed")]
        EmailConfirmed = 2,

        [UiInfo(Display = "Referral code")]
        ReferralCode = 3,

        [UiInfo(Display = "Daily Entry")]
        Daily = 4
    }
}