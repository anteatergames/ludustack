using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.Interfaces
{
    public interface IGiveawayScreenViewModel : IBaseViewModel
    {
        string StatusMessage { get; set; }
        GiveawayStatus Status { get; set; }
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }
        bool Future { get; set; }
        int SecondsToEnd { get; set; }
        bool CanCountDown { get; set; }
        bool CanReceiveEntries { get; set; }
        bool ShowTimeZone { get; set; }
        string TimeZone { get; set; }
        bool ShowSponsor { get; set; }
        string SponsorName { get; set; }
        string SponsorWebsite { get; set; }
        public string FeaturedImage { get; set; }
        List<string> ImageList { get; set; }
    }
}