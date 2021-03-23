using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class GetGameByIdQuery : GetByIdBaseQuery<Models.Game>
    {
        public GetGameByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetGameByIdQueryHandler : GetByIdBaseQueryHandler<GetGameByIdQuery, Models.Game, IGameRepository>
    {
        public GetGameByIdQueryHandler(IGameRepository repository) : base(repository)
        {
        }
    }
}