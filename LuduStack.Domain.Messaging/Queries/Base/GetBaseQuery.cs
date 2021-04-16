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
    public abstract class GetBaseQuery<TModel> : Query<IEnumerable<TModel>>
    {
        public Expression<Func<TModel, bool>> Where { get; set; }

        public int Take { get; set; }

        protected GetBaseQuery()
        {
        }

        protected GetBaseQuery(Expression<Func<TModel, bool>> where)
        {
            Where = where;
        }

        protected GetBaseQuery(Expression<Func<TModel, bool>> where, int take)
        {
            Where = where;
            Take = take;
        }
    }

    public abstract class GetBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, IEnumerable<TModel>> where TQuery : GetBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected GetBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<TModel>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                IQueryable<TModel> result = repository.Get(request.Where);

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                return result;
            }
            else
            {
                return await repository.GetAll();
            }
        }
    }
}