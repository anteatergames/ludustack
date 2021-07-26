using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;

namespace LuduStack.Domain.Messaging.Queries.Localization
{
    public class CountLocalizationQuery : CountBaseQuery<Models.Localization>
    {
    }

    public class CountLocalizationQueryHandler : CountBaseQueryHandler<CountLocalizationQuery, Models.Localization, ILocalizationRepository>
    {
        public CountLocalizationQueryHandler(ILocalizationRepository repository) : base(repository)
        {
        }
    }
}