using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetCoursesQuery : Query<List<StudyCourseListItemVo>>
    {
    }

    public class GetCoursesQueryHandler : QueryHandler, IRequestHandler<GetCoursesQuery, List<StudyCourseListItemVo>>
    {
        protected readonly IStudyCourseRepository studyCourseRepository;

        public GetCoursesQueryHandler(IStudyCourseRepository studyCourseRepository)
        {
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<List<StudyCourseListItemVo>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            List<StudyCourseListItemVo> objs = await studyCourseRepository.GetCourses();

            return objs;
        }
    }
}