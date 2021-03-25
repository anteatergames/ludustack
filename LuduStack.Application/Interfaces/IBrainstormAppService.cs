using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IBrainstormAppService : ICrudAppService<BrainstormIdeaViewModel>
    {
        OperationResultVo Vote(Guid userId, Guid ideaId, VoteValue vote);

        OperationResultVo Comment(CommentViewModel vm);

        Task<OperationResultVo<BrainstormSessionViewModel>> GetSession(Guid sessionId);

        OperationResultVo<BrainstormSessionViewModel> GetSession(Guid userId, BrainstormSessionType type);

        OperationResultListVo<BrainstormSessionViewModel> GetSessions(Guid userId);

        Task<OperationResultVo<Guid>> SaveSession(BrainstormSessionViewModel vm);

        OperationResultListVo<BrainstormIdeaViewModel> GetAllBySessionId(Guid userId, Guid sessionId);

        OperationResultVo<BrainstormSessionViewModel> GetMainSession();

        OperationResultVo ChangeStatus(Guid currentUserId, Guid ideaId, BrainstormIdeaStatus selectedStatus);
    }
}