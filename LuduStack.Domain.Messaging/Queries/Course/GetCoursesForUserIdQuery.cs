using LuduStack.Domain.ValueObjects.Study;
using System;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetCoursesForUserIdQuery : CourseQuery<StudyCoursesOfUserVo>
    {
        public Guid UserId { get; }

        public GetCoursesForUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}