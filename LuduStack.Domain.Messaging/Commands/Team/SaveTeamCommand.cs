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
    public class SaveTeamCommand : BaseUserCommand
    {
        public Team Team { get; }

        public SaveTeamCommand(Guid userId, Team team) : base(userId, team.Id)
        {
            Team = team;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveTeamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveTeamCommandHandler : CommandHandler, IRequestHandler<SaveTeamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ITeamRepository teamRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveTeamCommandHandler(IUnitOfWork unitOfWork, ITeamRepository teamRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.teamRepository = teamRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveTeamCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            if (request.Team.Id == Guid.Empty)
            {
                await teamRepository.Add(request.Team);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.TeamAdd);
            }
            else
            {
                teamRepository.Update(request.Team);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}