using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetPlansQuery : Query<List<StudyPlan>>
    {
        public Guid CourseId { get; }

        public GetPlansQuery(Guid courseId)
        {
            CourseId = courseId;
        }
    }

    public class GetPlansQueryHandler : QueryHandler, IRequestHandler<GetPlansQuery, List<StudyPlan>>
    {
        protected readonly IStudyCourseRepository studyCourseRepository;

        public GetPlansQueryHandler(IStudyCourseRepository studyCourseRepository)
        {
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<List<StudyPlan>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
        {
            List<StudyPlan> entries = await studyCourseRepository.GetPlans(request.CourseId);

            return entries;
        }
    }
}