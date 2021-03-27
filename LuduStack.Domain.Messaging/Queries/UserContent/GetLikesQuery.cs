using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetLikesQuery : Query<IEnumerable<UserContentLike>>
    {
        public Expression<Func<UserContentLike, bool>> Where { get; }

        public GetLikesQuery(Expression<Func<UserContentLike, bool>> where)
        {
            Where = where;
        }
    }

    public class GetLikesQueryHandler : QueryHandler, IRequestHandler<GetLikesQuery, IEnumerable<UserContentLike>>
    {
        protected readonly IUserContentRepository repository;

        public GetLikesQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<UserContentLike>> Handle(GetLikesQuery request, CancellationToken cancellationToken)
        {
            System.Linq.IQueryable<UserContentLike> comments = await repository.GetLikes(request.Where);

            return comments;
        }
    }
}