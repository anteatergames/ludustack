using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetUserProfileByUserIdQuery : GetByUserIdBaseQuery<Models.UserProfile>
    {
        public GetUserProfileByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetUserProfilesByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetUserProfileByUserIdQuery, Models.UserProfile, IUserProfileRepository>
    {
        public GetUserProfilesByUserIdQueryHandler(IUserProfileRepository repository) : base(repository)
        {
        }
    }
}