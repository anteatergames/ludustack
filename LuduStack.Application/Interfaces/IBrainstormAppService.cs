using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IBrainstormAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<BrainstormIdeaViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, BrainstormIdeaViewModel viewModel);

        Task<OperationResultVo> Vote(Guid userId, Guid ideaId, VoteValue voteValue);

        Task<OperationResultVo> Comment(Guid currentUserId, CommentViewModel vm);

        Task<OperationResultVo<BrainstormSessionViewModel>> GetSession(Guid sessionId);

        Task<OperationResultVo<BrainstormSessionViewModel>> GetSession(Guid userId, BrainstormSessionType type);

        Task<OperationResultListVo<BrainstormSessionViewModel>> GetSessions(Guid userId);

        Task<OperationResultVo<Guid>> SaveSession(Guid currentUserId, BrainstormSessionViewModel viewModel);

        Task<OperationResultListVo<BrainstormIdeaViewModel>> GetAllBySessionId(Guid userId, Guid sessionId, int? filter);

        Task<OperationResultVo<BrainstormSessionViewModel>> GetMainSession();

        Task<OperationResultVo> ChangeStatus(Guid currentUserId, Guid ideaId, BrainstormIdeaStatus selectedStatus);
    }
}