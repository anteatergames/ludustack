using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Localization
{
    public class GetLocalizationByIdQuery : GetByIdBaseQuery<Models.Localization>
    {
        public GetLocalizationByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetLocalizationByIdQueryHandler : GetByIdBaseQueryHandler<GetLocalizationByIdQuery, Models.Localization, ILocalizationRepository>
    {
        public GetLocalizationByIdQueryHandler(ILocalizationRepository repository) : base(repository)
        {
        }
    }
}