using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetPollsByUserContentIdsQuery : Query<IEnumerable<Poll>>
    {
        public List<Guid> UserContentIds { get; }

        public GetPollsByUserContentIdsQuery(IEnumerable<Guid> userContentIds)
        {
            UserContentIds = userContentIds.ToList();
        }
    }

    public class GetPollsByUserContentIdsQueryHandler : QueryHandler, IRequestHandler<GetPollsByUserContentIdsQuery, IEnumerable<Poll>>
    {
        private readonly IPollRepository pollRepository;

        public GetPollsByUserContentIdsQueryHandler(IPollRepository pollRepository)
        {
            this.pollRepository = pollRepository;
        }

        public async Task<IEnumerable<Poll>> Handle(GetPollsByUserContentIdsQuery request, CancellationToken cancellationToken)
        {
            return await pollRepository.GetPollsByUserContentIds(request.UserContentIds);
        }
    }
}