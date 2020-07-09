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
        public GiveawayStatus Status { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public string CoverImage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TimeZone { get; set; }

        public string StatusLocalized { get; set; }

        [Required]
        public bool MembersOnly { get; set; }

        [Required]
        public int WinnerAmount { get; set; }

        public int PrizePriceInDolar { get; set; }

        public string TermsAndConditions { get; set; }

        public string SponsorName { get; set; }

        public string SponsorWebsite { get; set; }

        public List<GiveawayPrizeViewModel> Prizes { get; set; }

        public List<GiveawayEntryOptionViewModel> EntryOptions { get; set; }

        public List<GiveawayParticipantViewModel> Participants { get; set; }

        #region Extra
        public bool CanCountDown { get; set; }

        public string StatusMessage { get; set; }

        public int SecondsToEnd { get; set; }

        public bool ShowSponsor { get; set; }

        public bool ShowTimeZone { get; set; }

        public bool Future { get; set; }
        #endregion
    }
}