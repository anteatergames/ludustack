using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Gamification
{
    public class GetGamificationByUserIdQuery : Query<Models.Gamification>
    {
        public Guid UserId { get; }

        public GetGamificationByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetGamificationByUserIdQueryHandler : QueryHandler, IRequestHandler<GetGamificationByUserIdQuery, Models.Gamification>
    {
        protected readonly IGamificationRepository repository;

        public GetGamificationByUserIdQueryHandler(IGamificationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Models.Gamification> Handle(GetGamificationByUserIdQuery request, CancellationToken cancellationToken)
        {
            System.Collections.Generic.IEnumerable<Models.Gamification> gamifications = await repository.GetByUserId(request.UserId);

            return gamifications.FirstOrDefault();
        }
    }
}