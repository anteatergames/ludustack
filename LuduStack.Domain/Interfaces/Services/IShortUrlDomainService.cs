using LuduStack.Domain.Models;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IShortUrlDomainService : IDomainService<ShortUrl>
    {
        string Add(string urlReferal, Core.Enums.ShortUrlDestinationType giveaway);

        ShortUrl GetByToken(string token);
    }
}