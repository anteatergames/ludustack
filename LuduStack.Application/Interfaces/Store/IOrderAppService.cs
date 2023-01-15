using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IOrderAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo<OrderListViewModel>> Get(Guid currentUserId);

        Task<OperationResultVo<OrderViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, OrderViewModel viewModel);

        Task<OperationResultVo> Sync(Guid currentUserId);
    }
}