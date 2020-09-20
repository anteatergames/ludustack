using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Study
{
    public class GetCourseByIdQueryHandler : QueryHandler, IRequestHandler<GetCourseByIdQuery, StudyCourse>
    {
        protected readonly IStudyCourseRepository studyCourseRepository;

        public GetCourseByIdQueryHandler(IStudyCourseRepository studyCourseRepository)
        {
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<StudyCourse> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await studyCourseRepository.GetById(request.Id);

            return course;
        }
    }
}