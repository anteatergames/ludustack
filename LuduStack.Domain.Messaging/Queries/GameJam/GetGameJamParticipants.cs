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
    public class GetGameJamParticipants : Query<IEnumerable<GameJamTeamMember>>
    {
        public Guid JamId { get; }

        public Expression<Func<Models.GameJamEntry, bool>> Where { get; }

        public GetGameJamParticipants(Guid jamId)
        {
            JamId = jamId;
        }

        public GetGameJamParticipants(Expression<Func<Models.GameJamEntry, bool>> where)
        {
            Where = where;
        }
    }

    public class GetGameJamParticipantsHandler : QueryHandler, IRequestHandler<GetGameJamParticipants, IEnumerable<GameJamTeamMember>>
    {
        protected readonly IGameJamEntryRepository repository;

        public GetGameJamParticipantsHandler(IGameJamEntryRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<GameJamTeamMember>> Handle(GetGameJamParticipants request, CancellationToken cancellationToken)
        {
            IEnumerable<GameJamTeamMember> list;

            if (request.Where != null)
            {
                IQueryable<GameJamTeamMember> participants = repository.GetParticipants(request.Where);

                list = participants.AsEnumerable();
            }
            else
            {
                IQueryable<GameJamEntry> query = repository.Get();

                query = Filter(request, query);

                IQueryable<GameJamTeamMember> projection = query.SelectMany(x => x.TeamMembers);

                list = projection.AsEnumerable();
            }

            IEnumerable<GameJamTeamMember> finalList = list.Distinct();

            return Task.FromResult(finalList);
        }

        private static IQueryable<GameJamEntry> Filter(GetGameJamParticipants request, IQueryable<GameJamEntry> query)
        {
            if (request.JamId != Guid.Empty)
            {
                query = query.Where(x => x.GameJamId == request.JamId);
            }

            return query;
        }
    }
}