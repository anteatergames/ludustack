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
    public class SaveGameJamCommand : BaseCommand
    {
        public GameJam GameJam { get; }

        public SaveGameJamCommand(GameJam gamificationLevel) : base(gamificationLevel.Id)
        {
            GameJam = gamificationLevel;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGameJamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGameJamCommandHandler : CommandHandler, IRequestHandler<SaveGameJamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamRepository forumGroupRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveGameJamCommandHandler(IUnitOfWork unitOfWork, IGameJamRepository forumGroupRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.forumGroupRepository = forumGroupRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveGameJamCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            request.GameJam.Handler = request.GameJam.Handler.ToLower();

            if (request.GameJam.Id == Guid.Empty)
            {
                await forumGroupRepository.Add(request.GameJam);
            }
            else
            {
                forumGroupRepository.Update(request.GameJam);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}