using LuduStack.Domain.Models;
using System;

namespace LuduStack.Domain.Messaging.Queries.Study
{
    public class GetCourseByIdQuery : CourseQuery<StudyCourse>
    {
        public Guid Id { get; }

        public GetCourseByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}