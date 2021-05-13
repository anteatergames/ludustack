using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Giveaway
{
    public class GetParticipantByEmailQuery : Query<GiveawayParticipant>
    {
        public Guid Id { get; }
        public string Email { get; }

        public GetParticipantByEmailQuery(Guid id, string email)
        {
            Id = id;
            Email = email;
        }
    }

    public class GetParticipantByEmailQueryHandler : QueryHandler, IRequestHandler<GetParticipantByEmailQuery, GiveawayParticipant>
    {
        private readonly IGiveawayRepository giveawayRepository;

        public GetParticipantByEmailQueryHandler(IGiveawayRepository giveawayRepository)
        {
            this.giveawayRepository = giveawayRepository;
        }

        public Task<GiveawayParticipant> Handle(GetParticipantByEmailQuery request, CancellationToken cancellationToken)
        {
            GiveawayParticipant model = giveawayRepository.GetParticipantByEmail(request.Id, request.Email);

            return Task.FromResult(model);
        }
    }
}