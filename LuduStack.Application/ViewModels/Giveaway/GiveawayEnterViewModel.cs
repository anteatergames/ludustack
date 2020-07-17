using System;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayEnterViewModel
    {
        public Guid GiveawayId { get; set; }

        public string Email { get; set; }

        public bool GdprConsent { get; set; }

        public bool WantNotifications { get; set; }

        public string ReferralCode { get; set; }
    }
}