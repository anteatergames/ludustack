using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Giveaway
{
    public class GetGiveawayListByUserIdQuery : Query<List<GiveawayListItemVo>>
    {
        public Guid UserId { get; }

        public GetGiveawayListByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetGiveawayListByUserIdHandler : QueryHandler, IRequestHandler<GetGiveawayListByUserIdQuery, List<GiveawayListItemVo>>
    {
        private readonly IGiveawayRepository giveawayRepository;
        private readonly IGiveawayDomainService giveawayDomainService;

        public GetGiveawayListByUserIdHandler(IGiveawayRepository giveawayRepository, IGiveawayDomainService giveawayDomainService)
        {
            this.giveawayRepository = giveawayRepository;
            this.giveawayDomainService = giveawayDomainService;
        }

        public Task<List<GiveawayListItemVo>> Handle(GetGiveawayListByUserIdQuery request, CancellationToken cancellationToken)
        {
            List<GiveawayListItemVo> giveaways = giveawayRepository.GetGiveawayListByUserId(request.UserId);

            foreach (GiveawayListItemVo item in giveaways)
            {
                giveawayDomainService.SetDates(item);
            }

            return Task.FromResult(giveaways);
        }
    }
}