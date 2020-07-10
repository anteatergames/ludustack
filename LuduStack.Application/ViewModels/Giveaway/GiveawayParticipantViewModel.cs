using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayParticipantViewModel : BaseViewModel
    {
        public string Email { get; set; }

        public string ReferalCode { get; set; }

        public bool IsWinner { get; set; }

        public string SecretReceived { get; set; }

        public List<GiveawayEntryViewModel> Entries { get; set; }
    }
}