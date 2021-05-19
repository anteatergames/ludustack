using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class UpdateParticipantShortUrlCommand : BaseCommand
    {
        public string Email { get; }
        public string ShortUrl { get; }

        public UpdateParticipantShortUrlCommand(Guid giveawayId, string email, string shortUrl) : base(giveawayId)
        {
            Email = email;
            ShortUrl = shortUrl;
        }

        public override bool IsValid()
        {
            Result.Validation = new UpdateParticipantShortUrlCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class UpdateParticipantShortUrlCommandHandler : CommandHandler, IRequestHandler<UpdateParticipantShortUrlCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGiveawayRepository giveawayRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public UpdateParticipantShortUrlCommandHandler(IUnitOfWork unitOfWork, IGiveawayRepository giveawayRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.giveawayRepository = giveawayRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(UpdateParticipantShortUrlCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            GiveawayParticipant existing = giveawayRepository.GetParticipantByEmail(request.Id, request.Email);

            if (existing != null)
            {
                existing.ShortUrl = request.ShortUrl;

                giveawayRepository.UpdateParticipant(request.Id, existing);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}