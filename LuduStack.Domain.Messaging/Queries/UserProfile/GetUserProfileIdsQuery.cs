using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetUserProfileIdsQuery : GetIdsBaseQuery<Models.UserProfile>
    {
        public GetUserProfileIdsQuery()
        {
        }

        public GetUserProfileIdsQuery(Expression<Func<Models.UserProfile, bool>> where) : base(where)
        {
        }
    }

    public class GetUserProfileIdsQueryHandler : GetIdsBaseQueryHandler<GetUserProfileIdsQuery, Models.UserProfile, IUserProfileRepository>
    {
        public GetUserProfileIdsQueryHandler(IUserProfileRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetUserProfileIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}