using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Base
{
    public abstract class GetBaseQuery<TModel> : Query<IEnumerable<TModel>>
    {
        public Expression<Func<TModel, bool>> Where { get; private set; }

        public GetBaseQuery()
        {
        }

        public GetBaseQuery(Expression<Func<TModel, bool>> where)
        {
            Where = where;
        }
    }

    public abstract class SearchBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, IEnumerable<TModel>> where TQuery : GetBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected SearchBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<TModel>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                return repository.Get(request.Where);
            }
            else
            {
                return await repository.GetAll();
            }
        }
    }
}