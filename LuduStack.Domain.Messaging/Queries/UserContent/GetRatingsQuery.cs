using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetRatingsQuery : Query<IEnumerable<UserContentRating>>
    {
        public Expression<Func<Models.UserContent, bool>> Where { get; }

        public GetRatingsQuery(Expression<Func<Models.UserContent, bool>> where)
        {
            Where = where;
        }
    }

    public class GetRatingsQueryHandler : QueryHandler, IRequestHandler<GetRatingsQuery, IEnumerable<UserContentRating>>
    {
        protected readonly IUserContentRepository repository;

        public GetRatingsQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<UserContentRating>> Handle(GetRatingsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<UserContentRating> comments = repository.GetRatings(request.Where);

            return Task.FromResult(comments.AsEnumerable());
        }
    }
}