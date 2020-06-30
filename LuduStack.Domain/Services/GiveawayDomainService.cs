using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;

namespace LuduStack.Domain.Services
{
    public class GiveawayDomainService : BaseDomainMongoService<Giveaway, IGiveawayRepository>, IGiveawayDomainService
    {
        public GiveawayDomainService(IGiveawayRepository repository) : base(repository)
        {
        }
    }
}
