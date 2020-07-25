using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Linq;

namespace LuduStack.Domain.Services
{
    public class ShortUrlDomainService : BaseDomainMongoService<ShortUrl, IShortUrlRepository>, IShortUrlDomainService
    {
        private static Random random = new Random();

        private const string BASEURL = "/go/";

        private const String ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public ShortUrlDomainService(IShortUrlRepository repository) : base(repository)
        {
        }

        private string Encode(int length)
        {
            var newToken = new string(Enumerable.Repeat(ALPHABET, length).Select(s => s[random.Next(s.Length)]).ToArray());

            return newToken;
        }

        public string Add(string urlReferal, ShortUrlDestinationType type)
        {
            var newToken = Encode(5);

            ShortUrl newShortUrl = new ShortUrl
            {
                OriginalUrl = urlReferal,
                Token = newToken,
                NewUrl = string.Format("{0}{1}", BASEURL, newToken),
                DestinationType = type
            };

            while (repository.CountDirectly(x => x.Token.Equals(newToken)) > 1)
            {
                newToken = Encode(5);
                newShortUrl.Token = newToken;
                newShortUrl.NewUrl = string.Format("{0}{1}", BASEURL, newToken);
            }

            repository.AddDirectly(newShortUrl);

            return newShortUrl.NewUrl;
        }

        public ShortUrl GetByToken(string token)
        {
            var obj = repository.Get().Where(x => x.Token.Equals(token));

            return obj.FirstOrDefault();
        }
    }
}