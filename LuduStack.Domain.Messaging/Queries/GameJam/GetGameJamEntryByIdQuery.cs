﻿using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamEntryByIdQuery : GetByIdBaseQuery<Models.GameJamEntry>
    {
        public string Handler { get; }

        public GetGameJamEntryByIdQuery(Guid id) : base(id)
        {
        }

        public GetGameJamEntryByIdQuery(Guid id, string handler) : this(id)
        {
            Handler = handler;
        }
    }

    public class GetGameJamEntryByIdQueryHandler : GetByIdBaseQueryHandler<GetGameJamEntryByIdQuery, Models.GameJamEntry, IGameJamEntryRepository>
    {
        public GetGameJamEntryByIdQueryHandler(IGameJamEntryRepository repository) : base(repository)
        {
        }

        public override async Task<Models.GameJamEntry> Handle(GetGameJamEntryByIdQuery request, CancellationToken cancellationToken)
        {
            Models.GameJamEntry obj;

            obj = await repository.GetById(request.Id);

            return obj;
        }
    }
}