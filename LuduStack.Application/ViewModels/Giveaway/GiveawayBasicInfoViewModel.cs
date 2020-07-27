using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayBasicInfoViewModel : UserGeneratedBaseViewModel
    {
        [Required]
        [Display(Name = "Status")]
        public GiveawayStatus Status { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        [Display(Name = "Images to show")]
        public List<string> ImageList { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        public string StatusLocalized { get; set; }

        [Required]
        [Display(Name = "Members Only")]
        public bool MembersOnly { get; set; }

        [Required]
        [Display(Name = "Winner Amount")]
        public int WinnerAmount { get; set; }

        [Display(Name = "Prize Price in Dolar")]
        public int PrizePriceInDolar { get; set; }

        [Display(Name = "Terms and Conditions")]
        public string TermsAndConditions { get; set; }

        [Display(Name = "Sponsor Name")]
        public string SponsorName { get; set; }

        [Display(Name = "Sponsor Website")]
        public string SponsorWebsite { get; set; }
    }
}