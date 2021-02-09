using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface ITeamAppService : ICrudAppService<TeamViewModel>
    {
        OperationResultVo<int> CountNotSingleMemberGroups(Guid currentUserId);

        OperationResultListVo<TeamViewModel> GetNotSingleMemberGroups(Guid currentUserId);

        OperationResultVo GenerateNewTeam(Guid currentUserId);

        OperationResultVo AcceptInvite(Guid teamId, Guid currentUserId, string quote);

        OperationResultVo RejectInvite(Guid teamId, Guid currentUserId);

        OperationResultVo GetByUserId(Guid userId);

        OperationResultVo GetSelectListByUserId(Guid userId);

        OperationResultVo RemoveMember(Guid currentUserId, Guid teamId, Guid userId);

        OperationResultVo CandidateApply(Guid currentUserId, TeamMemberViewModel vm);

        OperationResultVo AcceptCandidate(Guid currentUserId, Guid teamId, Guid userId);

        OperationResultVo RejectCandidate(Guid currentUserId, Guid teamId, Guid userId);
    }
}