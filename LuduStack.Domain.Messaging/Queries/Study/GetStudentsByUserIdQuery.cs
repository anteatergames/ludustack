using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Messaging.Queries.Study
{
    public class GetStudentsByUserIdQuery : CourseQuery<IEnumerable<Guid>>
    {
        public Guid UserId { get; }

        public GetStudentsByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}