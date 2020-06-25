using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;

namespace LuduStack.Domain.Services
{
    public class UserPreferencesDomainService : BaseDomainMongoService<UserPreferences, IUserPreferencesRepository>, IUserPreferencesDomainService
    {
        public UserPreferencesDomainService(IUserPreferencesRepository repository) : base(repository)
        {
        }
    }
}