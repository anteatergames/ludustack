using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class CountGameLikesQuery : GetByIdBaseQuery<int>
    {
        public CountGameLikesQuery(Guid id) : base(id)
        {
        }
    }

    public class CountGameLikesQueryHandler : QueryHandler, IRequestHandler<CountGameLikesQuery, int>
    {
        protected readonly IGameRepository repository;

        public CountGameLikesQueryHandler(IGameRepository repository)
        {
            this.repository = repository;
        }

        public async Task<int> Handle(CountGameLikesQuery request, CancellationToken cancellationToken)
        {
            int count = await repository.CountLikes(request.Id);

            return count;
        }
    }
}