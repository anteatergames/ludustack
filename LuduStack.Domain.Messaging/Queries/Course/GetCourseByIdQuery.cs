using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetCourseByIdQuery : GetByIdBaseQuery<Models.StudyCourse>
    {
        public GetCourseByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetCourseByIdQueryHandler : GetByIdBaseQueryHandler<GetCourseByIdQuery, Models.StudyCourse, IStudyCourseRepository>
    {
        public GetCourseByIdQueryHandler(IStudyCourseRepository repository) : base(repository)
        {
        }
    }
}