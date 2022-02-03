using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Team;
using LuduStack.Domain.Messaging.Queries.UserProfile;
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

        public TeamAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , ITeamDomainService teamDomainService
            , IGamificationDomainService gamificationDomainService) : base(profileBaseAppServiceCommon)
        {
            this.teamDomainService = teamDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

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

        public async Task<OperationResultListVo<TeamViewModel>> GetAll(Guid currentUserId, bool currentUserIsAdmin)
        {
            try
            {
                IEnumerable<Team> allModels = await mediator.Query<GetTeamQuery, IEnumerable<Team>>(new GetTeamQuery());

                List<TeamViewModel> vms = mapper.Map<IEnumerable<Team>, IEnumerable<TeamViewModel>>(allModels).ToList();

                IEnumerable<Guid> ids = vms.SelectMany(x => x.Members.Select(y => y.UserId));

                IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(ids));

                foreach (TeamViewModel team in vms)
                {
                    await SetUiData(currentUserId, currentUserIsAdmin, team, profiles);
                }

                vms = vms.OrderByDescending(x => x.CreateDate).ToList();

                return new OperationResultListVo<TeamViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<TeamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<TeamViewModel>> GetById(Guid currentUserId, bool currentUserIsAdmin, Guid id)
        {
            try
            {
                Team model = await mediator.Query<GetTeamByIdQuery, Team>(new GetTeamByIdQuery(id));

                if (model == null)
                {
                    return new OperationResultVo<TeamViewModel>("Team not found!");
                }

                TeamViewModel vm = mapper.Map<TeamViewModel>(model);

                List<Guid> ids = vm.Members.Select(y => y.UserId).ToList();
                ids.Add(currentUserId);

                IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(ids));

                vm.Members = vm.Members.OrderByDescending(x => x.Leader).ToList();
                foreach (TeamMemberViewModel member in vm.Members)
                {
                    UserProfileEssentialVo profile = profiles.FirstOrDefault(x => x.UserId == member.UserId);
                    member.Name = profile.Name;
                    member.UserHandler = profile.Handler;
                    member.Permissions.IsMe = member.UserId == currentUserId;
                    member.WorkDictionary = member.Works.ToDisplayNameList();
                }

                if (currentUserId != Guid.Empty)
                {
                    vm.CurrentUserIsMember = model.Members.Any(x => x.UserId == currentUserId);
                    vm.CurrentUserIsLeader = model.Members.Any(x => x.UserId == currentUserId && x.Leader);

                    if (vm.Recruiting)
                    {
                        UserProfileEssentialVo myProfile = profiles.FirstOrDefault(x => x.UserId == currentUserId);

                        vm.Candidate = new TeamMemberViewModel
                        {
                            UserId = currentUserId,
                            InvitationStatus = InvitationStatus.Candidate,
                            Name = myProfile.Name,
                            UserHandler = myProfile.Handler
                        };
                    }
                }

                await SetUiData(currentUserId, currentUserIsAdmin, vm, profiles);

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

                CommandResult result = await mediator.SendCommand(new SaveTeamCommand(currentUserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;

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
                await mediator.SendCommand(new DeleteTeamCommand(currentUserId, id));

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

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

        public async Task<OperationResultListVo<TeamViewModel>> GetNotSingleMemberGroups(Guid currentUserId, bool currentUserIsAdmin)
        {
            try
            {
                IEnumerable<Team> allModels = teamDomainService.GetNotSingleMemberGroups();

                IEnumerable<TeamViewModel> vms = mapper.Map<IEnumerable<Team>, IEnumerable<TeamViewModel>>(allModels);

                IEnumerable<Guid> ids = vms.SelectMany(x => x.Members.Select(y => y.UserId));

                IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(ids));

                foreach (TeamViewModel team in vms)
                {
                    await SetUiData(currentUserId, currentUserIsAdmin, team, profiles);
                }

                return new OperationResultListVo<TeamViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<TeamViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GenerateNewTeam(Guid currentUserId)
        {
            try
            {
                Team newTeam = teamDomainService.GenerateNewTeam(currentUserId);

                TeamViewModel newVm = mapper.Map<TeamViewModel>(newTeam);
                UserProfileEssentialVo myProfile = await GetCachedEssentialProfileByUserId(currentUserId);

                TeamMemberViewModel me = newVm.Members.FirstOrDefault(x => x.UserId == currentUserId);
                me.Name = myProfile.Name;
                me.UserHandler = myProfile.Handler;
                me.ProfileImage = UrlFormatter.ProfileImage(currentUserId);

                return new OperationResultVo<TeamViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> AcceptInvite(Guid teamId, Guid currentUserId, string quote)
        {
            int pointsEarned = 0;

            try
            {
                teamDomainService.ChangeInvitationStatus(teamId, currentUserId, InvitationStatus.Accepted, quote);

                pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.TeamJoin);

                await unitOfWork.Commit();

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

        public async Task<OperationResultListVo<TeamViewModel>> GetByUserId(Guid userId, bool currentUserIsAdmin)
        {
            try
            {
                IEnumerable<Team> allModels = teamDomainService.GetTeamsByMemberUserId(userId);

                IEnumerable<TeamViewModel> vms = mapper.Map<IEnumerable<Team>, IEnumerable<TeamViewModel>>(allModels);

                IEnumerable<Guid> ids = vms.SelectMany(x => x.Members.Select(y => y.UserId));

                IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(ids));

                foreach (TeamViewModel team in vms)
                {
                    await SetUiData(userId, currentUserIsAdmin, team, profiles);
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

                teamDomainService.AddMember(vm.TeamId, teamMemberModel);

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

                teamDomainService.AcceptCandidate(teamId, userId);

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

        private Task SetUiData(Guid userId, bool currentUserIsAdmin, TeamViewModel team, IEnumerable<UserProfileEssentialVo> profiles)
        {
            bool currentUserIsMember = team.Members.Any(x => x.UserId == userId);
            bool userIsLeader = team.Members.Any(x => x.Leader && x.UserId == userId);

            team.Permissions.IsMe = currentUserIsMember;
            team.Permissions.IsAdmin = currentUserIsAdmin;
            team.Permissions.CanEdit = userIsLeader;
            team.Permissions.CanDelete = currentUserIsAdmin || userIsLeader;
            team.Members = team.Members.OrderByDescending(x => x.Leader).ToList();

            foreach (TeamMemberViewModel member in team.Members)
            {
                UserProfileEssentialVo profile = profiles.FirstOrDefault(x => x.UserId == member.UserId);
                member.Permissions.CanDelete = member.UserId != userId && team.Permissions.CanDelete;
                member.ProfileImage = UrlFormatter.ProfileImage(member.UserId);
                member.UserHandler = profile.Handler;
            }

            if (team.Candidate != null)
            {
                team.Candidate.ProfileImage = UrlFormatter.ProfileImage(team.Candidate.UserId);
            }

            return Task.CompletedTask;
        }
    }
}