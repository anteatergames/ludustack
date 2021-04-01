using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserPreferences
{
    public class GetUserPreferencesByUserIdQuery : GetByUserIdBaseQuery<Models.UserPreferences>
    {
        public GetUserPreferencesByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetUserPreferencessByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetUserPreferencesByUserIdQuery, Models.UserPreferences, IUserPreferencesRepository>
    {
        public GetUserPreferencessByUserIdQueryHandler(IUserPreferencesRepository repository) : base(repository)
        {
        }
    }
}