using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class CheckGameJamHandlerQuery : Query<bool>
    {
        public string Handler { get; }

        public CheckGameJamHandlerQuery(string handler)
        {
            Handler = handler;
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
            var jam = gameJamRepository.Get(x => x.Handler.Equals(request.Handler));

            return Task.FromResult(!jam.Any());
        }
    }
}
