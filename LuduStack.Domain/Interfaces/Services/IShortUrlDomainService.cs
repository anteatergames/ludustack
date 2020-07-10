using LuduStack.Domain.Models;
using System;
using System.Linq;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IShortUrlDomainService : IDomainService<ShortUrl>
    {
        string Add(string urlReferal);
        ShortUrl GetByToken(string token);
    }
}