using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IUserPreferencesRepository : IRepository<UserPreferences>
    {
        Task<IEnumerable<SupportedLanguage>> GetUserLanguagesByUserId(Guid userId);
    }
}