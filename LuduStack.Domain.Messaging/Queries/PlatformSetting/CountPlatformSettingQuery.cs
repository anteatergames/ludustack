using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.PlatformSetting
{
    public class CountPlatformSettingQuery : CountBaseQuery<Models.PlatformSetting>
    {
        public CountPlatformSettingQuery()
        {
        }

        public CountPlatformSettingQuery(Expression<Func<Models.PlatformSetting, bool>> where) : base(where)
        {
        }
    }

    public class CountPlatformSettingQueryHandler : CountBaseQueryHandler<CountPlatformSettingQuery, Models.PlatformSetting, IPlatformSettingRepository>
    {
        public CountPlatformSettingQueryHandler(IPlatformSettingRepository repository) : base(repository)
        {
        }
    }
}