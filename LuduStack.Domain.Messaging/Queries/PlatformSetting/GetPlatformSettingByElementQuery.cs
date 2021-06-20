using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.PlatformSetting
{
    public class GetPlatformSettingByElementQuery : Query<Models.PlatformSetting>
    {
        public PlatformSettingElement Element { get; }

        public GetPlatformSettingByElementQuery(PlatformSettingElement element)
        {
            Element = element;
        }
    }

    public class GetPlatformSettingByElementQueryHandler : QueryHandler, IRequestHandler<GetPlatformSettingByElementQuery, Models.PlatformSetting>
    {
        protected readonly IPlatformSettingRepository platformSettingRepository;

        public GetPlatformSettingByElementQueryHandler(IPlatformSettingRepository platformSettingRepository)
        {
            this.platformSettingRepository = platformSettingRepository;
        }

        public Task<Models.PlatformSetting> Handle(GetPlatformSettingByElementQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.PlatformSetting> obj = platformSettingRepository.Get().Where(x => x.Element.Equals(request.Element));

            return Task.FromResult(obj.FirstOrDefault());
        }
    }
}