using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteTeamCommand : BaseUserCommand
    {
        public DeleteTeamCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteTeamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteTeamCommandHandler : CommandHandler, IRequestHandler<DeleteTeamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ITeamRepository teamRepository;
        protected readonly IGameRepository gameRepository;

        public DeleteTeamCommandHandler(IUnitOfWork unitOfWork, ITeamRepository teamRepository, IGameRepository gameRepository)
        {
            this.unitOfWork = unitOfWork;
            this.teamRepository = teamRepository;
            this.gameRepository = gameRepository;
        }

        public async Task<CommandResult> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.Team team = await teamRepository.GetById(request.Id);

            if (team is null)
            {
                AddError("The Team doesn't exist.");
                return request.Result;
            }

            List<Game> games = gameRepository.Get(x => x.TeamId == request.Id).ToList();

            foreach (Game game in games)
            {
                game.TeamId = null;
                gameRepository.Update(game);
            }

            // AddDomainEvent here

            teamRepository.Remove(team.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}