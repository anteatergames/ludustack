using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface ITeamAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo<TeamViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, TeamViewModel viewModel);

        Task<OperationResultListVo<TeamViewModel>> GetAll(Guid currentUserId);

        OperationResultVo<int> CountNotSingleMemberGroups(Guid currentUserId);

        OperationResultListVo<TeamViewModel> GetNotSingleMemberGroups(Guid currentUserId);

        Task<OperationResultVo> GenerateNewTeam(Guid currentUserId);

        OperationResultVo AcceptInvite(Guid teamId, Guid currentUserId, string quote);

        OperationResultVo RejectInvite(Guid teamId, Guid currentUserId);

        OperationResultVo GetByUserId(Guid userId);

        OperationResultVo GetSelectListByUserId(Guid userId);

        OperationResultVo RemoveMember(Guid currentUserId, Guid teamId, Guid userId);

        Task<OperationResultVo> CandidateApply(Guid currentUserId, TeamMemberViewModel vm);

        OperationResultVo AcceptCandidate(Guid currentUserId, Guid teamId, Guid userId);

        OperationResultVo RejectCandidate(Guid currentUserId, Guid teamId, Guid userId);
    }
}