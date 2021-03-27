using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IUserPreferencesAppService
    {
        Task<OperationResultVo<Guid>> Save(Guid currentUserId, UserPreferencesViewModel viewModel);

        UserPreferencesViewModel GetByUserId(Guid userId);
    }
}