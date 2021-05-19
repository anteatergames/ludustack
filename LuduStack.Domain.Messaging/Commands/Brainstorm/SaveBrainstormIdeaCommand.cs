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
    public class SaveBrainstormIdeaCommand : BaseUserCommand
    {
        public BrainstormIdea BrainstormIdea { get; }

        public SaveBrainstormIdeaCommand(Guid userId, BrainstormIdea brainstormIdea) : base(userId, brainstormIdea.Id)
        {
            UserId = userId;
            BrainstormIdea = brainstormIdea;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveBrainstormIdeaCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveBrainstormIdeaCommandHandler : CommandHandler, IRequestHandler<SaveBrainstormIdeaCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBrainstormIdeaRepository brainstormIdeaRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveBrainstormIdeaCommandHandler(IUnitOfWork unitOfWork, IBrainstormIdeaRepository brainstormIdeaRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.brainstormIdeaRepository = brainstormIdeaRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveBrainstormIdeaCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            if (request.BrainstormIdea.Id == Guid.Empty)
            {
                await brainstormIdeaRepository.Add(request.BrainstormIdea);

                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.IdeaSuggested);
            }
            else
            {
                await brainstormIdeaRepository.UpdateIdea(request.BrainstormIdea);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}