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
    public class GetMentorsByUserIdQuery : Query<IEnumerable<Guid>>
    {
        public Guid UserId { get; }

        public GetMentorsByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetMentorsByUserIdQueryHandler : QueryHandler, IRequestHandler<GetMentorsByUserIdQuery, IEnumerable<Guid>>
    {
        protected readonly IProfileDomainService profileDomainService;

        public GetMentorsByUserIdQueryHandler(IProfileDomainService profileDomainService)
        {
            this.profileDomainService = profileDomainService;
        }

        public Task<IEnumerable<Guid>> Handle(GetMentorsByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<UserConnection> mentorsIAdded = profileDomainService.GetConnectionByUserId(request.UserId, UserConnectionType.Mentor);
            IEnumerable<UserConnection> mentorsAddedMe = profileDomainService.GetConnectionByTargetUserId(request.UserId, UserConnectionType.Pupil);

            List<Guid> finalList = new List<Guid>();

            foreach (UserConnection connection in mentorsIAdded)
            {
                finalList.Add(connection.TargetUserId);
            }

            foreach (UserConnection mentor in mentorsAddedMe)
            {
                if (!finalList.Any(x => x == mentor.UserId))
                {
                    finalList.Add(mentor.UserId);
                }
            }

            return Task.FromResult(finalList.AsEnumerable());
        }
    }
}