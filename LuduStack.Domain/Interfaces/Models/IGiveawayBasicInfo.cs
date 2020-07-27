using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Models
{
    public interface IGiveawayBasicInfo
    {
        Guid Id { get; set; }

        Guid UserId { get; set; }

        DateTime CreateDate { get; set; }

        GiveawayStatus Status { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string FeaturedImage { get; set; }

        public List<string> ImageList { get; set; }

        DateTime StartDate { get; set; }

        DateTime? EndDate { get; set; }

        string TimeZone { get; set; }

        bool MembersOnly { get; set; }

        int WinnerAmount { get; set; }

        int PrizePriceInDolar { get; set; }

        string TermsAndConditions { get; set; }

        string SponsorName { get; set; }

        string SponsorWebsite { get; set; }
    }
}