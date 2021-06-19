using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.PlatformSetting
{
    public class GetPlatformSettingQuery : GetBaseQuery<Models.PlatformSetting>
    {
        public GetPlatformSettingQuery()
        {
        }

        public GetPlatformSettingQuery(Expression<Func<Models.PlatformSetting, bool>> where) : base(where)
        {
        }
    }

    public class GetPlatformSettingQueryHandler : GetBaseQueryHandler<GetPlatformSettingQuery, Models.PlatformSetting, IPlatformSettingRepository>
    {
        public GetPlatformSettingQueryHandler(IPlatformSettingRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.PlatformSetting>> Handle(GetPlatformSettingQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.PlatformSetting> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}