using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Giveaway
{
    public class CheckParticipantByEmailQuery : Query<bool>
    {
        public Guid Id { get; }
        public string Email { get; }

        public CheckParticipantByEmailQuery(Guid id, string email)
        {
            Id = id;
            Email = email;
        }
    }

    public class CheckParticipantByEmailQueryHandler : QueryHandler, IRequestHandler<CheckParticipantByEmailQuery, bool>
    {
        private readonly IGiveawayRepository giveawayRepository;

        public CheckParticipantByEmailQueryHandler(IGiveawayRepository giveawayRepository)
        {
            this.giveawayRepository = giveawayRepository;
        }

        public Task<bool> Handle(CheckParticipantByEmailQuery request, CancellationToken cancellationToken)
        {
            Guid guid = giveawayRepository.CheckParticipantByEmail(request.Id, request.Email);

            return Task.FromResult(guid != default);
        }
    }
}