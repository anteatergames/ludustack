using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects.Study;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetCoursesForUserIdQuery : Query<StudyCoursesOfUserVo>
    {
        public Guid UserId { get; }

        public GetCoursesForUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetCoursesForUserIdQueryHandler : QueryHandler, IRequestHandler<GetCoursesForUserIdQuery, StudyCoursesOfUserVo>
    {
        protected readonly IStudyCourseRepository studyCourseRepository;

        public GetCoursesForUserIdQueryHandler(IStudyCourseRepository studyCourseRepository)
        {
            this.studyCourseRepository = studyCourseRepository;
        }

        public Task<StudyCoursesOfUserVo> Handle(GetCoursesForUserIdQuery request, CancellationToken cancellationToken)
        {
            List<UserCourseVo> objs = studyCourseRepository.Get().Where(x => x.Members.Any(y => y.UserId == request.UserId)).Select(x => new UserCourseVo
            {
                CourseId = x.Id,
                CourseName = x.Name
            }).ToList();

            StudyCoursesOfUserVo result = new StudyCoursesOfUserVo
            {
                UserId = request.UserId
            };

            if (objs.Any())
            {
                result.Courses = objs;
            }

            return Task.FromResult(result);
        }
    }
}