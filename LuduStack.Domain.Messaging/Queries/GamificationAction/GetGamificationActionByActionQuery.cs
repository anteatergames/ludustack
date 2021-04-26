using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GamificationLevel
{
    public class GetGamificationActionByActionQuery : Query<Models.GamificationAction>
    {
        public PlatformAction Action { get; }

        public GetGamificationActionByActionQuery(PlatformAction action)
        {
            Action = action;
        }
    }

    public class GetGamificationActionByActionQueryHandler : QueryHandler, IRequestHandler<GetGamificationActionByActionQuery, Models.GamificationAction>
    {
        protected readonly IGamificationActionRepository repository;

        public GetGamificationActionByActionQueryHandler(IGamificationActionRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Models.GamificationAction> Handle(GetGamificationActionByActionQuery request, CancellationToken cancellationToken)
        {
            Models.GamificationAction obj = await repository.GetByAction(request.Action);

            return obj;
        }
    }
}