using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class CountUserProfileQuery : CountBaseQuery<Models.UserProfile>
    {
        public CountUserProfileQuery() : base()
        {
        }
    }
    public class CountUserProfileQueryHandler : CountBaseQueryHandler<CountUserProfileQuery, Models.UserProfile, IUserProfileRepository>
    {
        public CountUserProfileQueryHandler(IUserProfileRepository repository) : base(repository)
        {
        }
    }
}
