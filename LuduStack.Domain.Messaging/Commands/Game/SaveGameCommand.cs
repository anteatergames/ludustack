using FluentValidation.Results;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveGameCommand : BaseUserCommand
    {
        public Game Game { get; }
        public bool CreateTeam { get; }

        public SaveGameCommand(Guid userId, Game game, bool createTeam) : base(userId, game.Id)
        {
            Game = game;
            CreateTeam = createTeam;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGameCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGameCommandHandler : CommandHandler, IRequestHandler<SaveGameCommand, CommandResult>
    {
        private readonly IMediatorHandler mediator;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGameRepository gameRepository;
        private readonly IGamificationDomainService gamificationDomainService;
        private readonly ITeamDomainService teamDomainService;

        public SaveGameCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IGameRepository gameRepository, IGamificationDomainService gamificationDomainService, ITeamDomainService teamDomainService)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.gameRepository = gameRepository;
            this.gamificationDomainService = gamificationDomainService;
            this.teamDomainService = teamDomainService;
        }

        public async Task<CommandResult> Handle(SaveGameCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            Team newTeam = null;

            if (!request.IsValid()) { return request.Result; }

            if (request.CreateTeam)
            {
                newTeam = teamDomainService.GenerateNewTeam(request.UserId);
                newTeam.Name = String.Format("Team {0}", request.Game.Title);

                CommandResult saveTeamResult = await mediator.SendCommand(new SaveTeamCommand(request.UserId, newTeam));

                if (!saveTeamResult.Validation.IsValid)
                {
                    string saveTeam = saveTeamResult.Validation.Errors.FirstOrDefault().ErrorMessage;
                    result.Validation.Errors.Add(new ValidationFailure(string.Empty, saveTeam));
                }
                else
                {
                    request.Game.TeamId = newTeam.Id;
                    result.PointsEarned += saveTeamResult.PointsEarned;
                }
            }

            request.Game.ExternalLinks.RemoveAll(x => String.IsNullOrWhiteSpace(x.Value));

            if (request.Game.Id == Guid.Empty)
            {
                await gameRepository.Add(request.Game);
                result.PointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameAdd);
            }
            else
            {
                gameRepository.Update(request.Game);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}