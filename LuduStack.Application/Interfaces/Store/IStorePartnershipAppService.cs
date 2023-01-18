using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IStorePartnershipAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<StorePartnershipViewModel>> Get(Guid currentUserId);

        Task<OperationResultVo<StorePartnershipViewModel>> GetByPartner(Guid currentUserId, Guid userId);

        Task<OperationResultVo<StorePartnershipViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, StorePartnershipViewModel viewModel);

        Task<OperationResultVo> Sync(Guid currentUserId, Guid partnerUserId);
    }
}