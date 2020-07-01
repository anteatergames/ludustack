using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface ICrudAppService<T>
    {
        OperationResultVo<int> Count(Guid currentUserId);

        OperationResultListVo<T> GetAll(Guid currentUserId);

        OperationResultVo GetAllIds(Guid currentUserId);

        OperationResultVo<T> GetById(Guid currentUserId, Guid id);

        OperationResultVo Remove(Guid currentUserId, Guid id);

        OperationResultVo<Guid> Save(Guid currentUserId, T viewModel);
    }
}