using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class CheckGameJamHandlerQuery : Query<bool>
    {
        public string Handler { get; }
        public Guid Id { get; }

        public CheckGameJamHandlerQuery(string handler, Guid id)
        {
            Handler = handler;
            Id = id;
        }
    }

    public class CheckGameJamHandlerQueryHandler : QueryHandler, IRequestHandler<CheckGameJamHandlerQuery, bool>
    {
        private readonly IGameJamRepository gameJamRepository;

        public CheckGameJamHandlerQueryHandler(IGameJamRepository gameJamRepository)
        {
            this.gameJamRepository = gameJamRepository;
        }

        public Task<bool> Handle(CheckGameJamHandlerQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.GameJam> exists = gameJamRepository.Get(x => x.Handler.Equals(request.Handler) && x.Id != request.Id);

            return Task.FromResult(!exists.Any());
        }
    }
}