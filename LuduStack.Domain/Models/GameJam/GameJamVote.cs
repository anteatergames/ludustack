using System;

namespace LuduStack.Domain.Models
{
    public class GameJamVote
    {
        public Guid UserId { get; set; }

        public bool IsCommunityVote { get; set; }

        public Guid CriteriaId { get; set; }

        public decimal Score { get; set; }

        public string Comment { get; set; }
    }
}
