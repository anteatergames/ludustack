using LuduStack.Domain.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayEnterViewModel
    {
        public Guid GiveawayId { get; set; }

        [EmailAddress(ErrorMessage = "You must provide a valid email.")]
        [Required(ErrorMessage = "You must provide an email.")]
        public string Email { get; set; }

        public bool GdprConsent { get; set; }

        public bool WantNotifications { get; set; }

        public string ReferralCode { get; set; }

        public GiveawayEntryType? EntryType { get; set; }
    }
}