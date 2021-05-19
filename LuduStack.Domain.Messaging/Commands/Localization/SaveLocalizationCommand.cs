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
    public class SaveLocalizationCommand : BaseUserCommand
    {
        public Localization Localization { get; }

        public SaveLocalizationCommand(Guid userId, Localization jobPosition) : base(userId, jobPosition.Id)
        {
            Localization = jobPosition;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveLocalizationCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveLocalizationCommandHandler : CommandHandler, IRequestHandler<SaveLocalizationCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ILocalizationRepository jobPositionRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveLocalizationCommandHandler(IUnitOfWork unitOfWork, ILocalizationRepository jobPositionReopsitory, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            jobPositionRepository = jobPositionReopsitory;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveLocalizationCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            if (request.Localization.Id == Guid.Empty)
            {
                await jobPositionRepository.Add(request.Localization);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.LocalizationRequest);
            }
            else
            {
                jobPositionRepository.Update(request.Localization);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}