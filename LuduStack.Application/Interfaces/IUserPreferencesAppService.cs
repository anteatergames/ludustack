using LuduStack.Application.ViewModels.UserPreferences;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IUserPreferencesAppService : ICrudAppService<UserPreferencesViewModel>
    {
        UserPreferencesViewModel GetByUserId(Guid userId);
    }
}