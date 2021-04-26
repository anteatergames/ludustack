using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class GetGameByUserIdQuery : GetByUserIdBaseQuery<Models.Game>
    {
        public GetGameByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetGamesByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetGameByUserIdQuery, Models.Game, IGameRepository>
    {
        public GetGamesByUserIdQueryHandler(IGameRepository repository) : base(repository)
        {
        }
    }
}