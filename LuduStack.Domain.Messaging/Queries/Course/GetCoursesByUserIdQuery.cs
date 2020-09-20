using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetCoursesByUserIdQuery : CourseQuery<List<StudyCourseListItemVo>>
    {
        public Guid UserId { get; }

        public GetCoursesByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}