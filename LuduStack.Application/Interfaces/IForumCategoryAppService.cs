using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IForumCategoryAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<ForumCategoryViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultVo<ForumCategoryViewModel>> GetForDetails(Guid currentUserId, Guid id);

        Task<OperationResultVo<ForumCategoryViewModel>> GetForEdit(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, ForumCategoryViewModel viewModel);

        Task<OperationResultVo> GenerateNew(Guid currentUserId);
    }
}