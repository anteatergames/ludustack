using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface ICrudAppService<TViewModel>
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<TViewModel>> GetAll(Guid currentUserId);

        OperationResultVo GetAllIds(Guid currentUserId);

        Task<OperationResultVo<TViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, TViewModel viewModel);
    }
}