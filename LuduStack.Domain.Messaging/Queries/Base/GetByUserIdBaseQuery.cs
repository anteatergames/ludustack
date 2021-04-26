using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Base
{
    public abstract class GetByUserIdBaseQuery<TResult> : Query<IEnumerable<TResult>>
    {
        public Guid UserId { get; }

        protected GetByUserIdBaseQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public abstract class GetByUserIdBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, IEnumerable<TModel>> where TQuery : GetByUserIdBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected GetByUserIdBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual async Task<IEnumerable<TModel>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TModel> obj = await repository.GetByUserId(request.UserId);

            return obj;
        }
    }
}