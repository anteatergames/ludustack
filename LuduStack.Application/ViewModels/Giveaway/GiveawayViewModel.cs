using LuduStack.Domain.Core.Enums;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayViewModel : UserGeneratedBaseViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public string CoverImage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public GiveawayStatus Status { get; set; }
        public string StatusLocalized { get; set; }

        [Required]
        public bool MembersOnly { get; set; }

        [Required]
        public int WinnerAmount { get; set; }

        public string TermsAndConditions { get; set; }

        public List<GiveawayPrizeViewModel> Prizes { get; set; }

        public List<GiveawayEntryOptionViewModel> EntryOptions { get; set; }

        public List<GiveawayParticipantViewModel> Participants { get; set; }
    }
}