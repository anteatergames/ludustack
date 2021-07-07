using LuduStack.Domain.Interfaces.Repository;
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
    public class GetGameJamEntryListQuery : Query<IEnumerable<Models.GameJamEntry>>
    {
        public Expression<Func<Models.GameJamEntry, bool>> Where { get; set; }

        public Guid JamId { get; set; }

        public bool SubmittedOnly { get; set; }

        public int Take { get; set; }

        public GetGameJamEntryListQuery()
        {
        }

        public GetGameJamEntryListQuery(Guid jamId, bool submittedOnly)
        {
            JamId = jamId;
            SubmittedOnly = submittedOnly;
        }

        public GetGameJamEntryListQuery(Expression<Func<Models.GameJamEntry, bool>> where)
        {
            Where = where;
        }

        protected GetGameJamEntryListQuery(Expression<Func<Models.GameJamEntry, bool>> where, int take)
        {
            Where = where;
            Take = take;
        }
    }

    public class GetGameJamEntryListQueryHandler : QueryHandler, IRequestHandler<GetGameJamEntryListQuery, IEnumerable<Models.GameJamEntry>>
    {
        private readonly IGameJamEntryRepository repository;

        public GetGameJamEntryListQueryHandler(IGameJamEntryRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<Models.GameJamEntry>> Handle(GetGameJamEntryListQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                IQueryable<Models.GameJamEntry> result = repository.Get(request.Where);

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                return Task.FromResult(result.AsEnumerable());
            }
            else
            {
                IQueryable<Models.GameJamEntry> query = repository.Get();

                if (request.JamId != Guid.Empty)
                {
                    query = query.Where(x => x.GameJamId == request.JamId);
                }

                if (request.SubmittedOnly)
                {
                    query = query.Where(x => x.GameId.HasValue);
                }

                return Task.FromResult(query.AsEnumerable());
            }
        }
    }
}