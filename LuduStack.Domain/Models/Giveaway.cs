using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class Giveaway : Entity, IGiveawayBasicInfo
    {
        public GiveawayStatus Status { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public List<string> ImageList { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TimeZone { get; set; }

        public bool MembersOnly { get; set; }

        public int WinnerAmount { get; set; }

        public int PrizePriceInDolar { get; set; }

        public string TermsAndConditions { get; set; }

        public string SponsorName { get; set; }

        public string SponsorWebsite { get; set; }

        public List<GiveawayPrize> Prizes { get; set; }

        public List<GiveawayEntryOption> EntryOptions { get; set; }

        public List<GiveawayParticipant> Participants { get; set; }

        public Giveaway()
        {
            Prizes = new List<GiveawayPrize>();
            EntryOptions = new List<GiveawayEntryOption>();
            Participants = new List<GiveawayParticipant>();
        }
    }
}