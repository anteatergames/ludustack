using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Course;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class GetCoursesByUserIdQueryHandler : QueryHandler, IRequestHandler<GetCoursesByUserIdQuery, List<StudyCourseListItemVo>>
    {
        protected readonly IStudyCourseRepository studyCourseRepository;

        public GetCoursesByUserIdQueryHandler(IStudyCourseRepository studyCourseRepository)
        {
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<List<StudyCourseListItemVo>> Handle(GetCoursesByUserIdQuery message, CancellationToken cancellationToken)
        {
            List<StudyCourseListItemVo> objs = await studyCourseRepository.GetCoursesByUserId(message.UserId);

            return objs;
        }
    }
}