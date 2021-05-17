using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveGamificationCommand : BaseCommand
    {
        public Gamification Gamification { get; }

        public SaveGamificationCommand(Gamification gamification) : base(gamification.Id)
        {
            Gamification = gamification;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGamificationCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGamificationCommandHandler : CommandHandler, IRequestHandler<SaveGamificationCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGamificationRepository gamificationRepository;

        public SaveGamificationCommandHandler(IUnitOfWork unitOfWork, IGamificationRepository gamificationRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gamificationRepository = gamificationRepository;
        }

        public async Task<CommandResult> Handle(SaveGamificationCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.Gamification.Id == Guid.Empty)
            {
                await gamificationRepository.Add(request.Gamification);
            }
            else
            {
                gamificationRepository.Update(request.Gamification);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}