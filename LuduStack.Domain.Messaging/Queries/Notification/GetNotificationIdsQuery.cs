using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Notification
{
    public class GetNotificationIdsQuery : GetIdsBaseQuery<Models.Notification>
    {
        public GetNotificationIdsQuery()
        {
        }

        public GetNotificationIdsQuery(Expression<Func<Models.Notification, bool>> where) : base(where)
        {
        }
    }

    public class GetNotificationIdsQueryHandler : GetIdsBaseQueryHandler<GetNotificationIdsQuery, Models.Notification, INotificationRepository>
    {
        public GetNotificationIdsQueryHandler(INotificationRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetNotificationIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}