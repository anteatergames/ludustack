using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Linq;

namespace LuduStack.Domain.Services
{
    public class ShortUrlDomainService : IShortUrlDomainService
    {
        protected readonly IShortUrlRepository shortUrlRepository;

        private static Random random = new Random();

        private const string BASEURL = "/go/";

        private const String ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public ShortUrlDomainService(IShortUrlRepository shortUrlRepository)
        {
            this.shortUrlRepository = shortUrlRepository;
        }

        private string Encode(int length)
        {
            string newToken = new string(Enumerable.Repeat(ALPHABET, length).Select(s => s[random.Next(s.Length)]).ToArray());

            return newToken;
        }

        public string Add(string urlReferal, ShortUrlDestinationType type)
        {
            string newToken = Encode(5);

            ShortUrl newShortUrl = new ShortUrl
            {
                OriginalUrl = urlReferal,
                Token = newToken,
                NewUrl = string.Format("{0}{1}", BASEURL, newToken),
                DestinationType = type
            };

            while (shortUrlRepository.CountDirectly(x => x.Token.Equals(newToken)) > 1)
            {
                newToken = Encode(5);
                newShortUrl.Token = newToken;
                newShortUrl.NewUrl = string.Format("{0}{1}", BASEURL, newToken);
            }

            shortUrlRepository.AddDirectly(newShortUrl);

            return newShortUrl.NewUrl;
        }

        public ShortUrl GetByToken(string token)
        {
            IQueryable<ShortUrl> obj = shortUrlRepository.Get().Where(x => x.Token.Equals(token));

            return obj.FirstOrDefault();
        }
    }
}