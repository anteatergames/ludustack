using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class CountGameFollowersQuery : GetByIdBaseQuery<int>
    {
        public CountGameFollowersQuery(Guid id) : base(id)
        {
        }
    }

    public class CountGameFollowersQueryHandler : QueryHandler, IRequestHandler<CountGameFollowersQuery, int>
    {
        protected readonly IGameRepository repository;

        public CountGameFollowersQueryHandler(IGameRepository repository)
        {
            this.repository = repository;
        }

        public async Task<int> Handle(CountGameFollowersQuery request, CancellationToken cancellationToken)
        {
            int count = await repository.CountFollowers(request.Id);

            return count;
        }
    }
}