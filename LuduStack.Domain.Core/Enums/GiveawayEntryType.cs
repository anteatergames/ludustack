using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GiveawayEntryType
    {
        [Display(Name = "Login or email")]
        LoginOrEmail = 1,

        [Display(Name = "Facebook login")]
        FacebookLogin = 2,

        [Display(Name = "Facebook visit")]
        FacebookVisit = 3,

        [Display(Name = "Facebook share")]
        FacebookShare = 4,

        [Display(Name = "Twitter login")]
        TwitterLogin = 5,

        [Display(Name = "Twitter follow")]
        TwitterFollow = 6,

        [Display(Name = "Send tweet")]
        SendTweet = 7
    }
}