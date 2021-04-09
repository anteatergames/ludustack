using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IShortUrlDomainService
    {
        string Add(string urlReferal, ShortUrlDestinationType type);

        ShortUrl GetByToken(string token);
    }
}