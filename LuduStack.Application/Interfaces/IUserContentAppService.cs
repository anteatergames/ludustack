using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Search;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IUserContentAppService : IProfileBaseAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds();

        Task<OperationResultListVo<UserContentIdAndTypeVo>> GetAllContentIds();

        Task<OperationResultVo<UserContentViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, UserContentViewModel viewModel);

        Task<IEnumerable<UserContentViewModel>> GetActivityFeed(ActivityFeedRequestViewModel vm);

        Task<int> CountArticles();

        Task<OperationResultListVo<UserContentSearchViewModel>> Search(Guid currentUserId, string q);

        Task<OperationResultVo> ContentLike(Guid currentUserId, Guid targetId);

        Task<OperationResultVo> ContentUnlike(Guid currentUserId, Guid targetId);

        Task<OperationResultVo> Comment(Guid currentUserId, CommentViewModel vm);

        Task<OperationResultVo> GetCommentsByUserId(Guid currentUserId, Guid userId);
    }
}