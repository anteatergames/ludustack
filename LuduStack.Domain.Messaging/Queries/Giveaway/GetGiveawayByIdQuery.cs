using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Giveaway
{
    public class GetGiveawayByIdQuery : GetByIdBaseQuery<Models.Giveaway>
    {
        public GetGiveawayByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetGiveawayByIdQueryHandler : GetByIdBaseQueryHandler<GetGiveawayByIdQuery, Models.Giveaway, IGiveawayRepository>
    {
        public GetGiveawayByIdQueryHandler(IGiveawayRepository repository) : base(repository)
        {
        }
    }
}