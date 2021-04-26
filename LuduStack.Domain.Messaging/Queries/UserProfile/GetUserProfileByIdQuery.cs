using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetUserProfileByIdQuery : GetByIdBaseQuery<Models.UserProfile>
    {
        public GetUserProfileByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetUserProfileByIdQueryHandler : GetByIdBaseQueryHandler<GetUserProfileByIdQuery, Models.UserProfile, IUserProfileRepository>
    {
        public GetUserProfileByIdQueryHandler(IUserProfileRepository repository) : base(repository)
        {
        }
    }
}