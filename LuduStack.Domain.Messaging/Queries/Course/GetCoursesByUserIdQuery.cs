using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetCoursesByUserIdQuery : Query<List<StudyCourseListItemVo>>
    {
        public Guid UserId { get; }

        public GetCoursesByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetCoursesByUserIdQueryHandler : QueryHandler, IRequestHandler<GetCoursesByUserIdQuery, List<StudyCourseListItemVo>>
    {
        protected readonly IStudyCourseRepository studyCourseRepository;

        public GetCoursesByUserIdQueryHandler(IStudyCourseRepository studyCourseRepository)
        {
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<List<StudyCourseListItemVo>> Handle(GetCoursesByUserIdQuery request, CancellationToken cancellationToken)
        {
            List<StudyCourseListItemVo> objs = await studyCourseRepository.GetCoursesByUserId(request.UserId);

            return objs;
        }
    }
}