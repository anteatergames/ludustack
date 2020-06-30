using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public string CoverImage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public GiveawayStatus Status { get; set; }

        public bool MembersOnly { get; set; }

        public int WinnerAmount { get; set; }

        public List<GiveawayPrizeViewModel> Prizes { get; set; }

        public List<GiveawayEntryOptionViewModel> EntryOptions { get; set; }

        public List<GiveawayParticipantViewModel> Participants { get; set; }
    }
}
