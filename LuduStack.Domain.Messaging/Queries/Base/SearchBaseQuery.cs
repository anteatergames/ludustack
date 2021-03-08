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
    public abstract class SearchBaseQuery<TModel> : Query<IEnumerable<TModel>>
    {
        public Expression<Func<TModel, bool>> Where { get; private set; }


        public SearchBaseQuery(Expression<Func<TModel, bool>> where)
        {
            Where = where;
        }
    }

    public abstract class SearchBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, IEnumerable<TModel>> where TQuery : SearchBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected SearchBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<TModel>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TModel> objs = repository.Get(request.Where);

            return objs;
        }
    }
}
