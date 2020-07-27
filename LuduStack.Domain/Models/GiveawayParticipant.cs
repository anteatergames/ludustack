using LuduStack.Domain.Core.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class GiveawayParticipant : Entity
    {
        public string Email { get; set; }

        public string ReferralCode { get; set; }

        public string ShortUrl { get; set; }

        public bool GdprConsent { get; set; }

        public bool WantNotifications { get; set; }

        public bool IsWinner { get; set; }

        public string SecretReceived { get; set; }

        public List<GiveawayEntry> Entries { get; set; }

        public GiveawayParticipant()
        {
            Entries = new List<GiveawayEntry>();
        }
    }
}