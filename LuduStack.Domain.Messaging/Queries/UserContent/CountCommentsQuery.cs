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
    public class CountCommentsQuery : Query<int>
    {
        public Expression<Func<UserContentComment, bool>> Where { get; }

        public CountCommentsQuery(Expression<Func<UserContentComment, bool>> where)
        {
            Where = where;
        }
    }

    public class CountCommentsQueryHandler : QueryHandler, IRequestHandler<CountCommentsQuery, int>
    {
        protected readonly IUserContentRepository repository;

        public CountCommentsQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<int> Handle(CountCommentsQuery request, CancellationToken cancellationToken)
        {
            int count = await repository.CountComments(request.Where);

            return count;
        }
    }
}