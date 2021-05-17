using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Giveaway
{
    public class GetGiveawayBasicInfoByIdQuery : Query<GiveawayBasicInfoVo>
    {
        public Guid Id { get; }

        public GetGiveawayBasicInfoByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetGiveawayBasicInfoByIdQueryHandler : QueryHandler, IRequestHandler<GetGiveawayBasicInfoByIdQuery, GiveawayBasicInfoVo>
    {
        private readonly IGiveawayRepository giveawayRepository;
        private readonly IGiveawayDomainService giveawayDomainService;

        public GetGiveawayBasicInfoByIdQueryHandler(IGiveawayRepository giveawayRepository, IGiveawayDomainService giveawayDomainService)
        {
            this.giveawayRepository = giveawayRepository;
            this.giveawayDomainService = giveawayDomainService;
        }

        public async Task<GiveawayBasicInfoVo> Handle(GetGiveawayBasicInfoByIdQuery request, CancellationToken cancellationToken)
        {
            GiveawayBasicInfoVo model = await giveawayRepository.GetBasicGiveawayById(request.Id);

            if (model.Status == GiveawayStatus.Ended)
            {
                model.Winners = giveawayRepository.GetParticipants(request.Id).Where(x => x.IsWinner).ToList();
            }

            giveawayDomainService.SetDates(model);

            return model;
        }
    }
}