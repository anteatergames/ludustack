using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;

namespace LuduStack.Domain.Services
{
    public class UserPreferencesDomainService : IUserPreferencesDomainService
    {
        protected readonly IUserPreferencesRepository userPreferencesRepository;

        public UserPreferencesDomainService(IUserPreferencesRepository userPreferencesRepository)
        {
            this.userPreferencesRepository = userPreferencesRepository;
        }
    }
}