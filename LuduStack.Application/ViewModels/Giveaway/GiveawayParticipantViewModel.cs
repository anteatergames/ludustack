using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayParticipantViewModel : BaseViewModel
    {
        public string Email { get; set; }

        public string ReferralCode { get; set; }

        public string ShortUrl { get; set; }

        public bool IsWinner { get; set; }

        public string SecretReceived { get; set; }

        public List<GiveawayEntryViewModel> Entries { get; set; }
    }
}