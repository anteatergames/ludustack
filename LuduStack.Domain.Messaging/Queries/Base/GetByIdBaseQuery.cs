using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Base
{
    public abstract class GetByIdBaseQuery<TResult> : Query<TResult>
    {
        public Guid Id { get; }

        protected GetByIdBaseQuery(Guid id)
        {
            Id = id;
        }
    }

    public abstract class GetByIdBaseQueryHandler<TQuery, TModel, TRepository> : QueryHandler, IRequestHandler<TQuery, TModel> where TQuery : GetByIdBaseQuery<TModel> where TModel : Entity where TRepository : IRepository<TModel>
    {
        protected readonly TRepository repository;

        protected GetByIdBaseQueryHandler(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual async Task<TModel> Handle(TQuery request, CancellationToken cancellationToken)
        {
            TModel obj = await repository.GetById(request.Id);

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}