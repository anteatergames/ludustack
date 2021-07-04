using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Base
{
    public abstract class GetUserIdsBaseQuery<TModel> : Query<IEnumerable<Guid>>
    {
        public Expression<Func<TModel, bool>> Where { get; private set; }

        protected GetUserIdsBaseQuery()
        {
        }

        protected GetUserIdsBaseQuery(Expression<Func<TModel, bool>> where)
        {
            Where = where;
        }
    }

    public abstract class GetUserIdsBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, IEnumerable<Guid>> where TQuery : GetUserIdsBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected GetUserIdsBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<Guid>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                return Task.FromResult(repository.Get(request.Where).Select(x => x.UserId).AsEnumerable());
            }
            else
            {
                return Task.FromResult(repository.Get().Select(x => x.UserId).AsEnumerable());
            }
        }
    }
}