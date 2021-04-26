using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Study
{
    public class GetStudentsByUserIdQuery : Query<IEnumerable<Guid>>
    {
        public Guid UserId { get; }

        public GetStudentsByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetStudentsByUserIdQueryHandler : QueryHandler, IRequestHandler<GetStudentsByUserIdQuery, IEnumerable<Guid>>
    {
        protected readonly IProfileDomainService profileDomainService;

        public GetStudentsByUserIdQueryHandler(IProfileDomainService profileDomainService)
        {
            this.profileDomainService = profileDomainService;
        }

        public Task<IEnumerable<Guid>> Handle(GetStudentsByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<UserConnection> studentsIAdded = profileDomainService.GetConnectionByUserId(request.UserId, UserConnectionType.Pupil);
            IEnumerable<UserConnection> studentsAddedMe = profileDomainService.GetConnectionByTargetUserId(request.UserId, UserConnectionType.Mentor);

            List<Guid> finalList = new List<Guid>();

            foreach (UserConnection connection in studentsIAdded)
            {
                finalList.Add(connection.TargetUserId);
            }

            foreach (UserConnection student in studentsAddedMe)
            {
                if (!finalList.Any(x => x == student.UserId))
                {
                    finalList.Add(student.UserId);
                }
            }

            return Task.FromResult(finalList.AsEnumerable());
        }
    }
}