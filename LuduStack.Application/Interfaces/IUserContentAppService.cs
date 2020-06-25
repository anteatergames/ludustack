using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Search;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.Interfaces
{
    public interface IUserContentAppService : ICrudAppService<UserContentViewModel>, IProfileBaseAppService
    {
        IEnumerable<UserContentViewModel> GetActivityFeed(ActivityFeedRequestViewModel vm);

        int CountArticles();

        OperationResultListVo<UserContentSearchViewModel> Search(Guid currentUserId, string q);

        OperationResultVo ContentLike(Guid currentUserId, Guid targetId);

        OperationResultVo ContentUnlike(Guid currentUserId, Guid targetId);

        OperationResultVo Comment(CommentViewModel vm);
    }
}