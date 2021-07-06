using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamVoteViewModel
    {
        public Guid UserId { get; set; }

        public bool IsCommunityVote { get; set; }

        public GameJamCriteriaType CriteriaType { get; set; }

        public decimal Score { get; set; }

        public string Comment { get; set; }

        public decimal Median { get; set; }
    }
}