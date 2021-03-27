using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.GamificationLevel
{
    public class GetGamificationLevelByIdQuery : GetByIdBaseQuery<Models.GamificationLevel>
    {
        public GetGamificationLevelByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetGamificationLevelByIdQueryHandler : GetByIdBaseQueryHandler<GetGamificationLevelByIdQuery, Models.GamificationLevel, IGamificationLevelRepository>
    {
        public GetGamificationLevelByIdQueryHandler(IGamificationLevelRepository repository) : base(repository)
        {
        }
    }
}