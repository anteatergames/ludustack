using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetComicsByUserIdQuery : GetUserContentQuery
    {
        public GetComicsByUserIdQuery(Guid userId) : base(x => x.UserId == userId && x.UserContentType == UserContentType.ComicStrip)
        {
        }
    }

    public class GetComicsByUserIdQueryHandler : GetUserContentQueryHandler<GetComicsByUserIdQuery>
    {
        public GetComicsByUserIdQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }
    }

    public class GetUserContentQueryHandler<TQuery> : QueryHandler, IRequestHandler<TQuery, IEnumerable<Models.UserContent>> where TQuery : GetUserContentQuery
    {
        protected readonly IUserContentRepository repository;

        public GetUserContentQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.UserContent>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            System.Linq.IQueryable<Models.UserContent> comics = repository.Get(request.Where);

            return comics;
        }
    }
}