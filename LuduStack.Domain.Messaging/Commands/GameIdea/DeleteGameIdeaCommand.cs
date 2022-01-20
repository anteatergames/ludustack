using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteGameIdeaCommand : BaseUserCommand
    {
        public DeleteGameIdeaCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteGameIdeaCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteGameIdeaCommandHandler : CommandHandler, IRequestHandler<DeleteGameIdeaCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameIdeaRepository gamificationLevelRepository;

        public DeleteGameIdeaCommandHandler(IUnitOfWork unitOfWork, IGameIdeaRepository gamificationLevelRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gamificationLevelRepository = gamificationLevelRepository;
        }

        public async Task<CommandResult> Handle(DeleteGameIdeaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.GameIdea gamificationLevel = await gamificationLevelRepository.GetById(request.Id);

            if (gamificationLevel is null)
            {
                AddError("The Game Idea doesn't exist.");
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