using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamEntryVotesQuery : Query<IEnumerable<GameJamVote>>
    {
        public Expression<Func<Models.GameJamEntry, bool>> Where { get; }

        public GetGameJamEntryVotesQuery(Expression<Func<Models.GameJamEntry, bool>> where)
        {
            Where = where;
        }
    }

    public class GetGameJamEntryVotesQueryHandler : QueryHandler, IRequestHandler<GetGameJamEntryVotesQuery, IEnumerable<GameJamVote>>
    {
        protected readonly IGameJamEntryRepository repository;

        public GetGameJamEntryVotesQueryHandler(IGameJamEntryRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<GameJamVote>> Handle(GetGameJamEntryVotesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<GameJamVote> comments = repository.GetVotes(request.Where);

            return Task.FromResult(comments.AsEnumerable());
        }
    }
}