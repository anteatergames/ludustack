using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class TeamDomainService : ITeamDomainService
    {
        protected readonly ITeamRepository teamRepository;

        public TeamDomainService(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
        }

        public IQueryable<TeamMember> GetAllMembershipsByUser(Guid userId)
        {
            IQueryable<TeamMember> qry = teamRepository.GetMemberships(x => x.UserId == userId);

            return qry;
        }

        public TeamMember GetMemberByUserId(Guid teamId, Guid userId)
        {
            TeamMember obj = teamRepository.GetMembership(teamId, userId);

            return obj;
        }

        public void AcceptCandidate(Guid teamId, Guid userId)
        {
            ChangeInvitationStatus(teamId, userId, InvitationStatus.Accepted, null);
        }

        public void ChangeInvitationStatus(Guid teamId, Guid userId, InvitationStatus invitationStatus, string quote)
        {
            TeamMember member = teamRepository.GetMembership(teamId, userId);

            if (member != null)
            {
                member.InvitationStatus = invitationStatus;

                if (!string.IsNullOrWhiteSpace(quote))
                {
                    member.Quote = quote;
                }
            }

            teamRepository.UpdateMembership(teamId, member);
        }

        public void RemoveMember(Guid teamId, Guid userId)
        {
            TeamMember member = teamRepository.GetMembership(teamId, userId);

            if (member != null)
            {
                teamRepository.RemoveMember(teamId, userId);
            }
        }

        public void AddMember(Guid teamId, TeamMember newMember)
        {
            teamRepository.AddMember(teamId, newMember);
        }

        public IEnumerable<Team> GetTeamsByMemberUserId(Guid userId)
        {
            List<Team> teams = teamRepository.GetTeamsByMemberUserId(userId).ToList();

            return teams;
        }

        public IEnumerable<SelectListItemVo<Guid>> GetTeamListByMemberUserId(Guid userId)
        {
            var teams = teamRepository.GetTeamsByMemberUserId(userId).Select(x => new { x.Name, x.Id }).ToList();

            IEnumerable<SelectListItemVo<Guid>> vos = teams.Select(x => new SelectListItemVo<Guid>
            {
                Text = x.Name,
                Value = x.Id
            });

            return vos;
        }

        public Team GenerateNewTeam(Guid currentUserId)
        {
            Team team = new Team
            {
                Members = new List<TeamMember>()
            };

            TeamMember meAsMember = new TeamMember
            {
                UserId = currentUserId,
                Leader = true,
                InvitationStatus = InvitationStatus.Accepted
            };

            team.Members.Add(meAsMember);

            return team;
        }

        public int CountNotSingleMemberGroups()
        {
            Task<int> count = teamRepository.Count(x => x.Recruiting || x.Members.Count > 1);

            count.Wait();

            return count.Result;
        }

        public IEnumerable<Team> GetNotSingleMemberGroups()
        {
            IQueryable<Team> qry = teamRepository.Get(x => x.Recruiting || x.Members.Count > 1);

            return qry.OrderByDescending(x => x.CreateDate).ToList();
        }
    }
}