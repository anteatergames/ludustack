using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IUserPreferencesAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultVo> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<UserPreferencesViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, UserPreferencesViewModel viewModel);

        UserPreferencesViewModel GetByUserId(Guid userId);
    }
}