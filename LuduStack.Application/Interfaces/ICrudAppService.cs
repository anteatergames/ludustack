using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface ICrudAppService<TViewModel>
    {
        OperationResultVo<int> Count(Guid currentUserId);

        OperationResultListVo<TViewModel> GetAll(Guid currentUserId);

        OperationResultVo GetAllIds(Guid currentUserId);

        OperationResultVo<TViewModel> GetById(Guid currentUserId, Guid id);

        OperationResultVo Remove(Guid currentUserId, Guid id);

        OperationResultVo<Guid> Save(Guid currentUserId, TViewModel viewModel);
    }
}