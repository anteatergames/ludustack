using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetGamesByIdsQuery : Query<IEnumerable<Models.Game>>
    {
        public List<Guid> Ids { get; }

        public GetGamesByIdsQuery(IEnumerable<Guid> ids)
        {
            Ids = ids.ToList();
        }
    }

    public class GetGamesByIdsQueryHandler : QueryHandler, IRequestHandler<GetGamesByIdsQuery, IEnumerable<Models.Game>>
    {
        private readonly IGameRepository gameRepository;

        public GetGamesByIdsQueryHandler(IGameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public async Task<IEnumerable<Models.Game>> Handle(GetGamesByIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.Game> games = await gameRepository.GetByIds(request.Ids);

            return games;
        }
    }
}