using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IProductAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo<ProductListViewModel>> Get(Guid currentUserId);

        Task<OperationResultListVo<ProductViewModel>> GetByOwner(Guid currentUserId, Guid userId);

        Task<OperationResultVo<ProductViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, ProductViewModel viewModel);

        Task<OperationResultVo> Sync(Guid currentUserId);
    }
}