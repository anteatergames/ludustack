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
    public class CheckIfCommentExistsQuery : Query<bool>
    {
        public Expression<Func<UserContentComment, bool>> Where { get; }

        public CheckIfCommentExistsQuery(Expression<Func<UserContentComment, bool>> where)
        {
            Where = where;
        }
    }

    public class CheckIfCommentExistsQueryHandler : QueryHandler, IRequestHandler<CheckIfCommentExistsQuery, bool>
    {
        protected readonly IUserContentRepository repository;

        public CheckIfCommentExistsQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Handle(CheckIfCommentExistsQuery request, CancellationToken cancellationToken)
        {
            List<UserContentComment> task = await repository.GetComments(request.Where);

            bool exists = task.Any();

            return exists;
        }
    }
}