using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Localization
{
    public class GetLocalizationByUserIdQuery : GetByUserIdBaseQuery<Models.Localization>
    {
        public GetLocalizationByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetLocalizationsByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetLocalizationByUserIdQuery, Models.Localization, ILocalizationRepository>
    {
        public GetLocalizationsByUserIdQueryHandler(ILocalizationRepository repository) : base(repository)
        {
        }
    }
}