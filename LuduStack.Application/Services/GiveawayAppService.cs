using LuduStack.Application.Interfaces;

namespace LuduStack.Application.Services
{
    public class GiveawayAppService : ProfileBaseAppService, IGiveawayAppService
    {
        public GiveawayAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon)
        {
        }
    }
}
