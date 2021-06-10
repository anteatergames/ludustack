using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetUserProfileHandlersQuery : Query<IEnumerable<string>>
    {
        public Expression<Func<Models.UserProfile, bool>> Where { get; }

        public GetUserProfileHandlersQuery()
        {
        }

        public GetUserProfileHandlersQuery(Expression<Func<Models.UserProfile, bool>> where)
        {
            Where = where;
        }
    }

    public class GetUserProfileHandlersQueryHandler : QueryHandler, IRequestHandler<GetUserProfileHandlersQuery, IEnumerable<string>>
    {
        protected readonly IUserProfileRepository repository;

        public GetUserProfileHandlersQueryHandler(IUserProfileRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<string>> Handle(GetUserProfileHandlersQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                return Task.FromResult(repository.Get(request.Where).Select(x => x.Handler).AsEnumerable());
            }
            else
            {
                return Task.FromResult(repository.Get().Select(x => x.Handler).AsEnumerable());
            }
        }
    }
}