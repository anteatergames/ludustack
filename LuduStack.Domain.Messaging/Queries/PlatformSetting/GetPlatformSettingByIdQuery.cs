using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.PlatformSetting
{
    public class GetPlatformSettingByIdQuery : GetByIdBaseQuery<Models.PlatformSetting>
    {
        public GetPlatformSettingByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetPlatformSettingByIdQueryHandler : GetByIdBaseQueryHandler<GetPlatformSettingByIdQuery, Models.PlatformSetting, IPlatformSettingRepository>
    {
        public GetPlatformSettingByIdQueryHandler(IPlatformSettingRepository repository) : base(repository)
        {
        }
    }
}