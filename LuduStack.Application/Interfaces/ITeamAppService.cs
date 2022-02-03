using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface ITeamAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo<TeamViewModel>> GetById(Guid currentUserId, bool currentUserIsAdmin, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, TeamViewModel viewModel);

        Task<OperationResultListVo<TeamViewModel>> GetAll(Guid currentUserId, bool currentUserIsAdmin);

        OperationResultVo<int> CountNotSingleMemberGroups(Guid currentUserId);

        Task<OperationResultListVo<TeamViewModel>> GetNotSingleMemberGroups(Guid currentUserId, bool currentUserIsAdmin);

        Task<OperationResultVo> GenerateNewTeam(Guid currentUserId);

        Task<OperationResultVo> AcceptInvite(Guid teamId, Guid currentUserId, string quote);

        OperationResultVo RejectInvite(Guid teamId, Guid currentUserId);

        Task<OperationResultListVo<TeamViewModel>> GetByUserId(Guid userId, bool currentUserIsAdmin);

        OperationResultVo GetSelectListByUserId(Guid userId);

        OperationResultVo RemoveMember(Guid currentUserId, Guid teamId, Guid userId);

        Task<OperationResultVo> CandidateApply(Guid currentUserId, TeamMemberViewModel vm);

        OperationResultVo AcceptCandidate(Guid currentUserId, Guid teamId, Guid userId);

        OperationResultVo RejectCandidate(Guid currentUserId, Guid teamId, Guid userId);
    }
}