using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteGameJamCommand : BaseUserCommand
    {
        public DeleteGameJamCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteGameJamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteGameJamCommandHandler : CommandHandler, IRequestHandler<DeleteGameJamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamRepository gamificationLevelRepository;

        public DeleteGameJamCommandHandler(IUnitOfWork unitOfWork, IGameJamRepository gamificationLevelRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public async Task<CommandResult> Handle(DeleteGameJamCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.GameJam gamificationLevel = await gamificationLevelRepository.GetById(request.Id);

            if (gamificationLevel is null)
            {
                AddError("The Forum Group doesn't exist.");
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