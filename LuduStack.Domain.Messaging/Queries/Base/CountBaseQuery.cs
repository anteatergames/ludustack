using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Base
{
    public abstract class CountBaseQuery<TModel> : Query<int>
    {
        public Expression<Func<TModel, bool>> Where { get; private set; }

        protected CountBaseQuery()
        {
        }

        protected CountBaseQuery(Expression<Func<TModel, bool>> where)
        {
            Where = where;
        }
    }

    public abstract class CountBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, int> where TQuery : CountBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected CountBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public async Task<int> Handle(TQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                return await repository.Count(request.Where);
            }
            else
            {
                return await repository.Count();
            }
        }
    }
}