using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamVoteViewModel
    {
        public Guid UserId { get; set; }

        public bool IsCommunityVote { get; set; }

        public Guid CriteriaId { get; set; }

        public decimal Score { get; set; }

        public string Comment { get; set; }
    }
}
