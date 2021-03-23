using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetUserContentQuery : Query<IEnumerable<Models.UserContent>>
    {
        public Expression<Func<Models.UserContent, bool>> Where { get; }

        public GetUserContentQuery(Expression<Func<Models.UserContent, bool>> where)
        {
            Where = where;
        }
    }

    public class GetUserContentQueryHandler : GetUserContentQueryHandler<GetUserContentQuery>
    {
        public GetUserContentQueryHandler(IUserContentRepository repository) : base(repository)
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
            var comics = repository.Get(request.Where);

            return comics;
        }
    }
}