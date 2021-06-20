using LuduStack.Domain.Core.Attributes;
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
        ForumPageSize = 5
    }
}