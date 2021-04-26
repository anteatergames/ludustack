using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Notification
{
    public class GetNotificationByUserIdQuery : GetByUserIdBaseQuery<Models.Notification>
    {
        public GetNotificationByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetNotificationsByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetNotificationByUserIdQuery, Models.Notification, INotificationRepository>
    {
        public GetNotificationsByUserIdQueryHandler(INotificationRepository repository) : base(repository)
        {
        }
    }
}