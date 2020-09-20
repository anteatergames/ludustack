using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Messaging.Queries.Study
{
    public class GetMentorsByUserIdQuery : CourseQuery<IEnumerable<Guid>>
    {
        public Guid UserId { get; }

        public GetMentorsByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}