using LuduStack.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface ITeamRepository : IRepository<Team>
    {
        IQueryable<Team> GetTeamsByMemberUserId(Guid userId);

        IQueryable<TeamMember> GetMemberships(Func<TeamMember, bool> where);

        TeamMember GetMembership(Guid teamId, Guid userId);

        void UpdateMembership(Guid teamId, TeamMember member);

        Task<bool> RemoveMember(Guid teamId, Guid userId);

        Task AddMember(Guid teamId, TeamMember newMember);

        Task<bool> AddMemberDirectly(Guid teamId, TeamMember newMember);
    }
}