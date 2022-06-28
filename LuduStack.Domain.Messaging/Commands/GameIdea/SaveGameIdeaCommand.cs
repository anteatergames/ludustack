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
    public class SaveGameIdeaCommand : BaseCommand
    {
        public GameIdea GameIdea { get; }

        public SaveGameIdeaCommand(GameIdea gameIdea) : base(gameIdea.Id)
        {
            GameIdea = gameIdea;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGameIdeaCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGameIdeaCommandHandler : CommandHandler, IRequestHandler<SaveGameIdeaCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameIdeaRepository gameIdeaRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveGameIdeaCommandHandler(IUnitOfWork unitOfWork, IGameIdeaRepository gameIdeaRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.gameIdeaRepository = gameIdeaRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveGameIdeaCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            request.GameIdea.Description = request.GameIdea.Description.ToLower();

            if (request.GameIdea.Id == Guid.Empty)
            {
                await gameIdeaRepository.Add(request.GameIdea);
            }
            else
            {
                gameIdeaRepository.Update(request.GameIdea);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}