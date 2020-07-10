using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GiveawayEntryType
    {
        [Display(Name = "Login or email")]
        LoginOrEmail = 1,

        [Display(Name = "Email confirmed")]
        EmailConfirmed = 2,

        [Display(Name = "Referral code")]
        ReferralCode = 3
    }
}