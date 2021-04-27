using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IUserPreferencesAppService
    {
        Task<OperationResultVo<Guid>> Save(Guid currentUserId, UserPreferencesViewModel viewModel);

        Task<UserPreferencesViewModel> GetByUserId(Guid userId);

        Task<List<SupportedLanguage>> GetLanguagesByUserId(Guid userId);
    }
}