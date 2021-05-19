using LuduStack.Domain.Core.Enums;
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
    public class SaveGiveawayCommand : BaseUserCommand
    {
        public Giveaway Giveaway { get; }

        public SaveGiveawayCommand(Guid userId, Giveaway giveaway) : base(userId, giveaway.Id)
        {
            Giveaway = giveaway;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGiveawayCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGiveawayCommandHandler : CommandHandler, IRequestHandler<SaveGiveawayCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGiveawayRepository giveawayRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveGiveawayCommandHandler(IUnitOfWork unitOfWork, IGiveawayRepository giveawayRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.giveawayRepository = giveawayRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveGiveawayCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            SetRequiredProperties(request);

            if (request.Giveaway.Id == Guid.Empty)
            {
                await giveawayRepository.Add(request.Giveaway);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GiveawayAdd);
            }
            else
            {
                giveawayRepository.Update(request.Giveaway);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }

        private static void SetRequiredProperties(SaveGiveawayCommand request)
        {
            if (request.Giveaway.Status == 0)
            {
                request.Giveaway.Status = GiveawayStatus.Draft;
            }
        }
    }
}