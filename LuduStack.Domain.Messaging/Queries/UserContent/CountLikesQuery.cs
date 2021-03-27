using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class CountLikesQuery : Query<int>
    {
        public Expression<Func<UserContentLike, bool>> Where { get; }

        public CountLikesQuery(Expression<Func<UserContentLike, bool>> where)
        {
            Where = where;
        }
    }

    public class CountLikesQueryHandler : QueryHandler, IRequestHandler<CountLikesQuery, int>
    {
        protected readonly IUserContentRepository repository;

        public CountLikesQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<int> Handle(CountLikesQuery request, CancellationToken cancellationToken)
        {
            int count = await repository.CountLikes(request.Where);

            return count;
        }
    }
}