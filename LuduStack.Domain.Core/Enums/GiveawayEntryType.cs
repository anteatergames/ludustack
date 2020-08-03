using LuduStack.Domain.Core.Attributes;

namespace LuduStack.Domain.Core.Enums
{
    public enum GiveawayEntryType
    {
        [UiInfo(Display = "Participating", Description = "Participating")]
        LoginOrEmail = 1,

        [UiInfo(Display = "Email confirmation", Description = "Email confirmed")]
        EmailConfirmed = 2,

        [UiInfo(Display = "Referral code", Description = "Referral code")]
        ReferralCode = 3,

        [UiInfo(Display = "Daily Entry", Description = "Daily Entry")]
        Daily = 4,

        [UiInfo(Display = "Facebook Share", Description = "Share on Facebook")]
        FacebookShare = 5,

        [UiInfo(Display = "Twitter Share", Description = "Share on Twitter")]
        TwitterShare = 6
    }
}