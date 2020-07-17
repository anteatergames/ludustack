using LuduStack.Domain.Models;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IShortUrlDomainService : IDomainService<ShortUrl>
    {
        string Add(string urlReferal);

        ShortUrl GetByToken(string token);
    }
}