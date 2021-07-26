using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Domain.Services
{
    public class GameJamDomainService : IGameJamDomainService
    {
        public void CheckTeamMembers(GameJamEntry entry, IEnumerable<Guid> userIds)
        {
            if (userIds != null && userIds.Any())
            {
                if (entry.TeamMembers == null)
                {
                    entry.TeamMembers = new List<GameJamTeamMember>();
                }

                foreach (Guid newUserId in userIds)
                {
                    if (!entry.TeamMembers.Any(x => x.UserId == newUserId))
                    {
                        GameJamTeamMember newTeamMember = new GameJamTeamMember { UserId = newUserId, TeamJoinDate = DateTime.Now, IsSubmitter = entry.UserId == newUserId };

                        entry.TeamMembers.Add(newTeamMember);
                    }
                }

                entry.TeamMembers = entry.TeamMembers.Where(x => userIds.Contains(x.UserId)).ToList();

                entry.IsTeam = entry.TeamMembers.Count > 1;
            }
        }

        public void CheckTeamMembers(Models.GameJamEntry obj)
        {
            if (obj.TeamMembers == null || !obj.TeamMembers.Any())
            {
                Models.GameJamTeamMember meTeamMember = new Models.GameJamTeamMember
                {
                    UserId = obj.UserId
                };

                obj.TeamMembers = new List<Models.GameJamTeamMember> { meTeamMember };
            }
        }
    }
}