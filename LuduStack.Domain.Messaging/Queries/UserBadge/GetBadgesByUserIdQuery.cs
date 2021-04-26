using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserBadge
{
    public class GetBadgesByUserIdQuery : GetByUserIdBaseQuery<Models.UserBadge>
    {
        public GetBadgesByUserIdQuery(Guid userId) : base(userId)
        {
        }
    }

    public class GetBadgesByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetBadgesByUserIdQuery, Models.UserBadge, IUserBadgeRepository>
    {
        public GetBadgesByUserIdQueryHandler(IUserBadgeRepository repository) : base(repository)
        {
        }
    }
}