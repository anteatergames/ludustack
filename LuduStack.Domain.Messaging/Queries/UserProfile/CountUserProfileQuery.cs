using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class CountUserProfileQuery : CountBaseQuery<Models.UserProfile>
    {
    }

    public class CountUserProfileQueryHandler : CountBaseQueryHandler<CountUserProfileQuery, Models.UserProfile, IUserProfileRepository>
    {
        public CountUserProfileQueryHandler(IUserProfileRepository repository) : base(repository)
        {
        }
    }
}