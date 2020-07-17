using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayBasicInfoViewModel : UserGeneratedBaseViewModel
    {
        [Required]
        public GiveawayStatus Status { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        [Display(Name = "Images to show")]
        public List<string> ImageList { get; set; }

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
    }
}