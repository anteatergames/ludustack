using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Notification
{
    public class GetNotificationByIdQuery : GetByIdBaseQuery<Models.Notification>
    {
        public GetNotificationByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetNotificationByIdQueryHandler : GetByIdBaseQueryHandler<GetNotificationByIdQuery, Models.Notification, INotificationRepository>
    {
        public GetNotificationByIdQueryHandler(INotificationRepository repository) : base(repository)
        {
        }
    }
}