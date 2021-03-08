using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class GetGamesByUserIdQuery : GetByUserIdBaseQuery<Models.Game>
    {
        public GetGamesByUserIdQuery(Guid id) : base(id)
        {
        }
    }
    public class GetGamesByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetGamesByUserIdQuery, Models.Game, IGameRepository>
    {
        public GetGamesByUserIdQueryHandler(IGameRepository repository) : base(repository)
        {
        }
    }
}
