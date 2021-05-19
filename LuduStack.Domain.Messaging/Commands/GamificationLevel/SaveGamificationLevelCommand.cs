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
    public class SaveGamificationLevelCommand : BaseCommand
    {
        public GamificationLevel GamificationLevel { get; }

        public SaveGamificationLevelCommand(GamificationLevel gamificationLevel) : base(gamificationLevel.Id)
        {
            GamificationLevel = gamificationLevel;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGamificationLevelCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGamificationLevelCommandHandler : CommandHandler, IRequestHandler<SaveGamificationLevelCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGamificationLevelRepository gamificationLevelRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveGamificationLevelCommandHandler(IUnitOfWork unitOfWork, IGamificationLevelRepository gamificationLevelRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.gamificationLevelRepository = gamificationLevelRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveGamificationLevelCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.GamificationLevel.Id == Guid.Empty)
            {
                await gamificationLevelRepository.Add(request.GamificationLevel);
            }
            else
            {
                gamificationLevelRepository.Update(request.GamificationLevel);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}