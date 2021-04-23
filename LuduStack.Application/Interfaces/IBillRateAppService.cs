using LuduStack.Application.ViewModels.BillRate;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IBillRateAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<BillRateViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, BillRateViewModel viewModel);

        Task<OperationResultVo> GetByMe(Guid currentUserId);

        OperationResultVo GenerateNew(Guid currentUserId);

        Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid id);
    }
}