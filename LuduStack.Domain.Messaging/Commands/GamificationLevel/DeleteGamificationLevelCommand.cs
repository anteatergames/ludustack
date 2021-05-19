using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteGamificationLevelCommand : BaseUserCommand
    {
        public DeleteGamificationLevelCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteGamificationLevelCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteGamificationLevelCommandHandler : CommandHandler, IRequestHandler<DeleteGamificationLevelCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGamificationLevelRepository gamificationLevelRepository;

        public DeleteGamificationLevelCommandHandler(IUnitOfWork unitOfWork, IGamificationLevelRepository gamificationLevelRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public async Task<CommandResult> Handle(DeleteGamificationLevelCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.GamificationLevel gamificationLevel = await gamificationLevelRepository.GetById(request.Id);

            if (gamificationLevel is null)
            {
                AddError("The Gamification Level doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            gamificationLevelRepository.Remove(gamificationLevel.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}