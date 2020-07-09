using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayEnterViewModel
    {
        public Guid GiveawayId { get; set; }

        public string Email { get; set; }

        public bool GdprConsent { get; set; }

        public bool WantNotifications { get; set; }
    }
}
