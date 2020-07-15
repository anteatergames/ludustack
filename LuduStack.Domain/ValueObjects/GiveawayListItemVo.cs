using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class GiveawayListItemVo : IGiveawayBasicInfo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public GiveawayStatus Status { get; set; }

        public string FeaturedImage { get; set; }

        public List<string> ImageList { get; set; }

        public string Description { get; set; }

        public string CoverImage { get; set; }

        public string TimeZone { get; set; }

        public bool MembersOnly { get; set; }

        public int WinnerAmount { get; set; }

        public int PrizePriceInDolar { get; set; }

        public string TermsAndConditions { get; set; }

        public string SponsorName { get; set; }

        public string SponsorWebsite { get; set; }

        public int ParticipantCount { get; set; }

        public string StatusLocalized { get; set; }
    }
}