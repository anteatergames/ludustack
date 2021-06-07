using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IForumAppService
    {
        Task<OperationResultVo<ForumPostViewModel>> GenerateNewTopic(Guid currentUserId, Guid? categoryId);

        Task<OperationResultVo<ForumIndexViewModel>> GetAllCategoriesByGroup(Guid currentUserId);

        Task<OperationResultListVo<ForumCategoryListItemVo>> GetAllCategories(Guid currentUserId);

        Task<OperationResultVo<ForumCategoryViewModel>> GetCategory(Guid currentUserId, Guid id, string handler);

        Task<OperationResultVo<ForumPostListVo>> GetPosts(Guid currentUserId, GetForumPostsRequestViewModel viewModel);

        Task<OperationResultVo<Guid>> SavePost(Guid currentUserId, ForumPostViewModel viewModel);

        Task<OperationResultVo<ForumPostViewModel>> GetPostForDetails(Guid currentUserId, Guid id);

        Task<OperationResultVo<ForumPostViewModel>> GetPostForEdit(Guid currentUserId, Guid id);

        Task<OperationResultListVo<ForumPostViewModel>> GetTopicAnswers(Guid currentUserId, GetForumTopicAnswersRequestViewModel viewModel);

        Task<OperationResultVo<ForumPostViewModel>> RemovePost(Guid currentUserId, Guid id);

        Task<OperationResultVo<int>> Vote(Guid currentUserId, Guid postId, VoteValue vote);
    }
}