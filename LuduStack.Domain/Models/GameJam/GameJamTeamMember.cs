using System;

namespace LuduStack.Domain.Models
{
    public class GameJamTeamMember
    {
        public bool IsSubmitter { get; set; }

        public Guid UserId { get; set; }

        public DateTime TeamJoinDate { get; set; }
    }
}