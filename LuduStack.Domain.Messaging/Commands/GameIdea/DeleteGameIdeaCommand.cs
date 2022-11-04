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
        protected readonly IGameIdeaRepository gameIdeaRepository;

        public DeleteGameIdeaCommandHandler(IUnitOfWork unitOfWork, IGameIdeaRepository gameIdeaRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gameIdeaRepository = gameIdeaRepository;
        }

        public async Task<CommandResult> Handle(DeleteGameIdeaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.GameIdea gameIdea = await gameIdeaRepository.GetById(request.Id);

            if (gameIdea is null)
            {
                AddError("The Game Idea doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            gameIdeaRepository.Remove(gameIdea.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}