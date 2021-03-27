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
    public class GetCommentsQuery : Query<IEnumerable<UserContentComment>>
    {
        public Expression<Func<UserContentComment, bool>> Where { get; }

        public GetCommentsQuery(Expression<Func<UserContentComment, bool>> where)
        {
            Where = where;
        }
    }

    public class GetCommentsQueryHandler : QueryHandler, IRequestHandler<GetCommentsQuery, IEnumerable<UserContentComment>>
    {
        protected readonly IUserContentRepository repository;

        public GetCommentsQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<UserContentComment>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            List<UserContentComment> comments = await repository.GetComments(request.Where);

            return comments;
        }
    }
}