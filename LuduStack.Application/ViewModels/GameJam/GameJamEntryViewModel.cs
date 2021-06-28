using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamEntryViewModel : BaseViewModel
    {
        public bool LateSubmission { get; set; }

        public bool IsTeam { get; set; }

        public Guid GameJamId { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime DeliverDate { get; set; }

        public Guid GameId { get; set; }

        public Guid? TeamId { get; set; }

        public int FinalPlace { get; set; }

        public List<GameJamVoteViewModel> Votes { get; set; }
    }
}
