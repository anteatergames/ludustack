﻿using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamEntryQuery : Query<Models.GameJamEntry>
    {
        public Guid UserId { get; set; }

        public Guid JamId { get; set; }

        public Expression<Func<Models.GameJamEntry, bool>> Where { get; set; }

        public int Take { get; set; }

        public GetGameJamEntryQuery()
        {
        }

        public GetGameJamEntryQuery(Guid userId, Guid jamId)
        {
            UserId = userId;
            JamId = jamId;
        }

        public GetGameJamEntryQuery(Expression<Func<Models.GameJamEntry, bool>> where)
        {
            Where = where;
        }

        protected GetGameJamEntryQuery(Expression<Func<Models.GameJamEntry, bool>> where, int take)
        {
            Where = where;
            Take = take;
        }
    }

    public class GetGameJamEntryQueryHandler : QueryHandler, IRequestHandler<GetGameJamEntryQuery, Models.GameJamEntry>
    {
        private readonly IGameJamEntryRepository repository;

        public GetGameJamEntryQueryHandler(IGameJamEntryRepository repository)
        {
            this.repository = repository;
        }

        public Task<Models.GameJamEntry> Handle(GetGameJamEntryQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                IQueryable<Models.GameJamEntry> items = repository.Get();

                if (request.Take > 0)
                {
                    items = items.Take(request.Take);
                }

                return Task.FromResult(items.FirstOrDefault());
            }
            else
            {
                IQueryable<Models.GameJamEntry> items = repository.Get(x => x.UserId == request.UserId && x.GameJamId == request.JamId);

                return Task.FromResult(items.FirstOrDefault());
            }
        }
    }
}