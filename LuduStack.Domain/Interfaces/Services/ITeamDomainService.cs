using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface ITeamDomainService
    {
        TeamMember GetMemberByUserId(Guid teamId, Guid userId);

        IQueryable<TeamMember> GetAllMembershipsByUser(Guid userId);

        void AcceptCandidate(Guid teamId, Guid userId);

        void ChangeInvitationStatus(Guid teamId, Guid userId, InvitationStatus invitationStatus, string quote);

        void RemoveMember(Guid teamId, Guid userId);

        void AddMember(Guid teamId, TeamMember newMember);

        IEnumerable<Team> GetTeamsByMemberUserId(Guid userId);

        IEnumerable<SelectListItemVo<Guid>> GetTeamListByMemberUserId(Guid userId);

        Team GenerateNewTeam(Guid currentUserId);

        int CountNotSingleMemberGroups();

        IEnumerable<Team> GetNotSingleMemberGroups();
    }
}