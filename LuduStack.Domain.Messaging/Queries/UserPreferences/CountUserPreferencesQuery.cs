using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;

namespace LuduStack.Domain.Messaging.Queries.UserPreferences
{
    public class CountUserPreferencesQuery : CountBaseQuery<Models.UserPreferences>
    {
    }

    public class CountUserPreferencesQueryHandler : CountBaseQueryHandler<CountUserPreferencesQuery, Models.UserPreferences, IUserPreferencesRepository>
    {
        public CountUserPreferencesQueryHandler(IUserPreferencesRepository repository) : base(repository)
        {
        }
    }
}