using System;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamTeamMemberViewModel
    {
        public bool IsSubmitter { get; set; }

        public Guid UserId { get; set; }

        public DateTime TeamJoinDate { get; set; }
    }
}