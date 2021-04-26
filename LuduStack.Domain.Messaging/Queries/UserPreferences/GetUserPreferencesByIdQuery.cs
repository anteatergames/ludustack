using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserPreferences
{
    public class GetUserPreferencesByIdQuery : GetByIdBaseQuery<Models.UserPreferences>
    {
        public GetUserPreferencesByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetUserPreferencesByIdQueryHandler : GetByIdBaseQueryHandler<GetUserPreferencesByIdQuery, Models.UserPreferences, IUserPreferencesRepository>
    {
        public GetUserPreferencesByIdQueryHandler(IUserPreferencesRepository repository) : base(repository)
        {
        }
    }
}