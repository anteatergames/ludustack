using LuduStack.Domain.Core.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class GiveawayParticipant : Entity
    {
        public string Email { get; set; }

        public string RefferalCode { get; set; }

        public bool IsWinner { get; set; }

        public string SecretReceived { get; set; }

        public List<GiveawayEntry> Entries { get; set; }
    }
}