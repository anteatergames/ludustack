using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamWinnersQuery : Query<IEnumerable<Models.GameJamEntry>>
    {
        public Guid JamId { get; set; }

        public int WinnerCount { get; set; }

        public GetGameJamWinnersQuery()
        {
        }

        public GetGameJamWinnersQuery(Guid jamId, int winnerCount)
        {
            JamId = jamId;
            WinnerCount = winnerCount;
        }
    }

    public class GetGameJamWinnersQueryHandler : QueryHandler, IRequestHandler<GetGameJamWinnersQuery, IEnumerable<Models.GameJamEntry>>
    {
        private readonly IGameJamEntryRepository repository;

        public GetGameJamWinnersQueryHandler(IGameJamEntryRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<Models.GameJamEntry>> Handle(GetGameJamWinnersQuery request, CancellationToken cancellationToken)
        {
            if (request.WinnerCount > 0)
            {
                IQueryable<Models.GameJamEntry> result = repository.Get(x => x.FinalPlace <= request.WinnerCount);

                return Task.FromResult(result.AsEnumerable());
            }
            else
            {
                List<Models.GameJamEntry> emptyList = new List<Models.GameJamEntry>();

                return Task.FromResult(emptyList.AsEnumerable());
            }
        }
    }
}