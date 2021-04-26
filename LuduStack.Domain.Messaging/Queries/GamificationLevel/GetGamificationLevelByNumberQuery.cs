using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GamificationLevel
{
    public class GetGamificationLevelByNumberQuery : Query<Models.GamificationLevel>
    {
        public int Number { get; }

        public GetGamificationLevelByNumberQuery(int number)
        {
            Number = number;
        }
    }

    public class GetGamificationLevelByNumberQueryHandler : QueryHandler, IRequestHandler<GetGamificationLevelByNumberQuery, Models.GamificationLevel>
    {
        protected readonly IGamificationLevelRepository repository;

        public GetGamificationLevelByNumberQueryHandler(IGamificationLevelRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Models.GamificationLevel> Handle(GetGamificationLevelByNumberQuery request, CancellationToken cancellationToken)
        {
            Models.GamificationLevel obj = await repository.GetByNumber(request.Number);

            return obj;
        }
    }
}