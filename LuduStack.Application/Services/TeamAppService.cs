using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.Team;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class TeamAppService : ProfileBaseAppService, ITeamAppService
    {
        private readonly ITeamDomainService teamDomainService;
        private readonly IGamificationDomainService gamificationDomainService;

        public TeamAppService(IMediatorHandler mediator
            , IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , ITeamDomainService teamDomainService
            , IGamificationDomainService gamificationDomainService) : base(mediator, profileBaseAppServiceCommon)
        {
            this.teamDomainService = teamDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

        #region ICrudAppService

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountTeamQuery, int>(new CountTeamQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<TeamViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<Team> allModels = await mediator.Query<GetTeamQuery, IEnumerable<Team>>(new GetTeamQuery());

                IEnumerable<TeamViewModel> vms = mapper.Map<IEnumerable<Team>, IEnumerable<TeamViewModel>>(allModels);

                foreach (TeamViewModel team in vms)
                {
                    SetUiData(currentUserId, team);
                }

                return new OperationResultListVo<TeamViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<TeamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = teamDomainService.GetAllIds();

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<TeamViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                Team model = await mediator.Query<GetTeamByIdQuery, Team>(new GetTeamByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<TeamViewModel>("Team not found!");
                }

                TeamViewModel vm = mapper.Map<TeamViewModel>(model);

                vm.Members = vm.Members.OrderByDescending(x => x.Leader).ToList();
                foreach (TeamMemberViewModel member in vm.Members)
                {
                    UserProfile profile = GetCachedProfileByUserId(member.UserId);
                    member.Name = profile.Name;
                    member.Permissions.IsMe = member.UserId == currentUserId;
                    member.WorkDictionary = member.Works.ToDisplayNameList();
                }

                if (currentUserId != Guid.Empty)
                {
                    vm.CurrentUserIsMember = model.Members.Any(x => x.UserId == currentUserId);

                    if (vm.Recruiting)
                    {
                        UserProfile myProfile = GetCachedProfileByUserId(currentUserId);

                        vm.Candidate = new TeamMemberViewModel
                        {
                            UserId = currentUserId,
                            InvitationStatus = InvitationStatus.Candidate,
                            Name = myProfile.Name
                        };
                    }
                }

                SetUiData(currentUserId, vm);

                return new OperationResultVo<TeamViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<TeamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, TeamViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                Team model;

                Team existing = await mediator.Query<GetTeamByIdQuery, Team>(new GetTeamByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Team>(viewModel);
                }

                if (viewModel.Id == Guid.Empty)
                {
                    teamDomainService.Add(model);
                    viewModel.Id = model.Id;

                    pointsEarned += gamificationDomainService.ProcessAction(viewModel.UserId, PlatformAction.TeamAdd);
                }
                else
                {
                    teamDomainService.Update(model);
                }

                await unitOfWork.Commit();

                viewModel.Id = model.Id;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                // validate before

                teamDomainService.Remove(id);

                await unitOfWork.Commit();

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #endregion ICrudAppService

        #region ITeamAppService

        public OperationResultVo<int> CountNotSingleMemberGroups(Guid currentUserId)
        {
            try
            {
                int count = teamDomainService.CountNotSingleMemberGroups();

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public OperationResultListVo<TeamViewModel> GetNotSingleMemberGroups(Guid currentUserId)
        {
            try
            {
                IEnumerable<Team> allModels = teamDomainService.GetNotSingleMemberGroups();

                IEnumerable<TeamViewModel> vms = mapper.Map<IEnumerable<Team>, IEnumerable<TeamViewModel>>(allModels);

                foreach (TeamViewModel team in vms)
                {
                    SetUiData(currentUserId, team);
                }

                return new OperationResultListVo<TeamViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<TeamViewModel>(ex.Message);
            }
        }

        public OperationResultVo GenerateNewTeam(Guid currentUserId)
        {
            try
            {
                Team newTeam = teamDomainService.GenerateNewTeam(currentUserId);

                TeamViewModel newVm = mapper.Map<TeamViewModel>(newTeam);
                UserProfile myProfile = GetCachedProfileByUserId(currentUserId);

                TeamMemberViewModel me = newVm.Members.FirstOrDefault(x => x.UserId == currentUserId);
                me.Name = myProfile.Name;
                me.ProfileImage = UrlFormatter.ProfileImage(currentUserId);

                return new OperationResultVo<TeamViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo AcceptInvite(Guid teamId, Guid currentUserId, string quote)
        {
            int pointsEarned = 0;

            try
            {
                teamDomainService.ChangeInvitationStatus(teamId, currentUserId, InvitationStatus.Accepted, quote);

                pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.TeamJoin);

                unitOfWork.Commit();

                return new OperationResultVo(true, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo RejectInvite(Guid teamId, Guid currentUserId)
        {
            try
            {
                teamDomainService.RemoveMember(teamId, currentUserId);

                unitOfWork.Commit();

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetByUserId(Guid userId)
        {
            try
            {
                IEnumerable<Team> allModels = teamDomainService.GetTeamsByMemberUserId(userId);

                IEnumerable<TeamViewModel> vms = mapper.Map<IEnumerable<Team>, IEnumerable<TeamViewModel>>(allModels);

                foreach (TeamViewModel team in vms)
                {
                    SetUiData(userId, team);
                }

                return new OperationResultListVo<TeamViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<TeamViewModel>(ex.Message);
            }
        }

        public OperationResultVo GetSelectListByUserId(Guid userId)
        {
            try
            {
                IEnumerable<SelectListItemVo<Guid>> allModels = teamDomainService.GetTeamListByMemberUserId(userId);

                IEnumerable<SelectListItemVo> list = allModels.Select(x => new SelectListItemVo
                {
                    Text = x.Text,
                    Value = x.Value.ToString()
                });

                return new OperationResultListVo<SelectListItemVo>(list);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo RemoveMember(Guid currentUserId, Guid teamId, Guid userId)
        {
            try
            {
                // validate before

                teamDomainService.RemoveMember(teamId, userId);

                unitOfWork.Commit();

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> CandidateApply(Guid currentUserId, TeamMemberViewModel vm)
        {
            int pointsEarned = 0;

            try
            {
                Team team = await mediator.Query<GetTeamByIdQuery, Team>(new GetTeamByIdQuery(vm.TeamId));

                if (team == null)
                {
                    return new OperationResultVo("Team not found!");
                }

                TeamMember teamMemberModel = mapper.Map<TeamMember>(vm);

                team.Members.Add(teamMemberModel);

                pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.TeamJoin);

                await unitOfWork.Commit();

                return new OperationResultVo(true, "Application sent! Now just sit and wait the team leader to accept you.", pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo AcceptCandidate(Guid currentUserId, Guid teamId, Guid userId)
        {
            try
            {
                TeamMember member = teamDomainService.GetMemberByUserId(teamId, userId);

                member.InvitationStatus = InvitationStatus.Accepted;

                unitOfWork.Commit();

                return new OperationResultVo(true, "Member accepted!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo RejectCandidate(Guid currentUserId, Guid teamId, Guid userId)
        {
            try
            {
                teamDomainService.RemoveMember(teamId, userId);

                unitOfWork.Commit();

                return new OperationResultVo(true, "Member accepted!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #endregion ITeamAppService

        private void SetUiData(Guid userId, TeamViewModel team)
        {
            bool userIsLeader = team.Members.Any(x => x.Leader && x.UserId == userId);

            team.Permissions.CanEdit = userIsLeader && team.Members.Any(x => x.UserId == userId && x.Leader);
            team.Permissions.CanDelete = userIsLeader && team.Members.Any(x => x.UserId == userId && x.Leader);
            team.Members = team.Members.OrderByDescending(x => x.Leader).ToList();

            foreach (TeamMemberViewModel member in team.Members)
            {
                member.Permissions.CanDelete = member.UserId != userId && team.Permissions.CanDelete;
                member.ProfileImage = UrlFormatter.ProfileImage(member.UserId);
            }

            if (team.Candidate != null)
            {
                team.Candidate.ProfileImage = UrlFormatter.ProfileImage(team.Candidate.UserId);
            }
        }
    }
}