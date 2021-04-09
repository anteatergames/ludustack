using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ShortUrl
{
    public class GetShortUrlByTokenQuery : Query<Models.ShortUrl>
    {
        public string Token { get; }

        public GetShortUrlByTokenQuery(string token)
        {
            Token = token;
        }
    }

    public class GetShortUrlByTokenQueryHandler : QueryHandler, IRequestHandler<GetShortUrlByTokenQuery, Models.ShortUrl>
    {
        protected readonly IShortUrlRepository shortUrlRepository;

        public GetShortUrlByTokenQueryHandler(IShortUrlRepository shortUrlRepository)
        {
            this.shortUrlRepository = shortUrlRepository;
        }

        public Task<Models.ShortUrl> Handle(GetShortUrlByTokenQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.ShortUrl> obj = shortUrlRepository.Get().Where(x => x.Token.Equals(request.Token));

            return Task.FromResult(obj.FirstOrDefault());
        }
    }
}