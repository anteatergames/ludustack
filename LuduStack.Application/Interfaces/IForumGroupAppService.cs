using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IForumGroupAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<IEnumerable<SelectListItemVo>> GetSelectList(Guid userId);

        Task<OperationResultListVo<ForumGroupViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultVo<ForumGroupViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, ForumGroupViewModel viewModel);

        Task<OperationResultVo> GenerateNew(Guid currentUserId);
    }
}