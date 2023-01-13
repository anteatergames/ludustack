using LuduStack.Domain.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum PlatformSettingElement
    {
        [UiInfo(Display = "Show Ads", Type = (int)PlatformSettingType.Boolean, SubType = (int)PlatformSettingGroup.General, DefaultValue = "1")]
        [Display(Name = "Show Ads")]
        ShowAds = 1,

        [UiInfo(Display = "Show Feature Carousel", Type = (int)PlatformSettingType.Boolean, SubType = (int)PlatformSettingGroup.Home, DefaultValue = "1")]
        [Display(Name = "Show Feature Carousel")]
        ShowFeatureCarousel = 2,

        [UiInfo(Display = "Show Donate Button", Type = (int)PlatformSettingType.Boolean, SubType = (int)PlatformSettingGroup.Home, DefaultValue = "1")]
        [Display(Name = "Show Donate Button")]
        ShowDonateButton = 3,

        [UiInfo(Display = "Feed Page Size", Type = (int)PlatformSettingType.Integer, SubType = (int)PlatformSettingGroup.Feed, DefaultValue = "10")]
        [Display(Name = "Feed Page Size")]
        FeedPageSize = 4,

        [UiInfo(Display = "Forum Page Size", Type = (int)PlatformSettingType.Integer, SubType = (int)PlatformSettingGroup.Forum, DefaultValue = "20")]
        [Display(Name = "Forum Page Size")]
        ForumPageSize = 5,

        [UiInfo(Display = "Partnership Minimum Withdraw", Type = (int)PlatformSettingType.Integer, SubType = (int)PlatformSettingGroup.Store, DefaultValue = "100")]
        [Display(Name = "Partnership Minimum Withdraw")]
        PartnershipMinimumWithdraw = 6,

        [UiInfo(Display = "Show HomePage Idea Generator", Type = (int)PlatformSettingType.Boolean, SubType = (int)PlatformSettingGroup.Feature, DefaultValue = "1")]
        [Display(Name = "Show HomePage Idea Generator")]
        ShowHomePageIdeaGenerator = 7,

        [UiInfo(Display = "Show Store", Type = (int)PlatformSettingType.Boolean, SubType = (int)PlatformSettingGroup.Feature, DefaultValue = "1")]
        [Display(Name = "Show Store")]
        ShowStore = 8,

        [UiInfo(Display = "Show Games", Type = (int)PlatformSettingType.Boolean, SubType = (int)PlatformSettingGroup.Feature, DefaultValue = "1")]
        [Display(Name = "Show Games")]
        ShowGames = 9,

        [UiInfo(Display = "Facebook Url", Type = (int)PlatformSettingType.Text, SubType = (int)PlatformSettingGroup.General, DefaultValue = "")]
        [Display(Name = "Facebook Url")]
        FacebookUrl = 10,

        [UiInfo(Display = "Discord Url", Type = (int)PlatformSettingType.Text, SubType = (int)PlatformSettingGroup.General, DefaultValue = "")]
        [Display(Name = "Discord Url")]
        DiscordUrl = 11,

        [UiInfo(Display = "Twitter Url", Type = (int)PlatformSettingType.Text, SubType = (int)PlatformSettingGroup.General, DefaultValue = "")]
        [Display(Name = "Twitter Url")]
        TwitterUrl = 12,
    }
}