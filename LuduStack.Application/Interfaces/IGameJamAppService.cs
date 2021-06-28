using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IGameJamAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<IEnumerable<SelectListItemVo>> GetSelectList(Guid userId);

        Task<OperationResultListVo<GameJamViewModel>> GetAll(Guid currentUserId);

        Task<OperationResultListVo<GameJamViewModel>> GetByUserId(Guid userId);

        Task<OperationResultVo<GameJamViewModel>> GetForDetails(Guid currentUserId, Guid id, string handler);

        Task<OperationResultVo<GameJamViewModel>> GetForEdit(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, GameJamViewModel viewModel);

        Task<OperationResultVo<GameJamViewModel>> GenerateNew(Guid currentUserId);

        Task<OperationResultVo> ValidateHandler(Guid currentUserId, string handler);
    }
}
