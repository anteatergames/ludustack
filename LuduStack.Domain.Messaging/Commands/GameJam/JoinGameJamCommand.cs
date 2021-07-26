using FluentValidation.Results;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
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
    public class JoinGameJamCommand : BaseUserCommand
    {
        public JoinGameJamCommand(Guid userId, Guid jamId) : base(userId, jamId)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new JoinGameJamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class JoinGameJamCommandHandler : CommandHandler, IRequestHandler<JoinGameJamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamRepository gameJamRepository;
        protected readonly IGameJamEntryRepository entryRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public JoinGameJamCommandHandler(IUnitOfWork unitOfWork, IGameJamRepository gameJamRepository, IGameJamEntryRepository entryRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.gameJamRepository = gameJamRepository;
            this.entryRepository = entryRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(JoinGameJamCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            IQueryable<GameJamEntry> existing = entryRepository.Get(x => x.UserId == request.UserId && x.GameJamId == request.Id);

            if (existing.Any())
            {
                result.Validation.Errors.Add(new ValidationFailure(string.Empty, "You have already joined this Game Jam!"));
            }
            else
            {
                GameJamEntry newEntry = GenerateNewEntry(request.UserId, request.Id);

                await entryRepository.Add(newEntry);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamJoin);

                result.Validation = await Commit(unitOfWork);
                result.PointsEarned = pointsEarned;
            }

            return result;
        }

        private GameJamEntry GenerateNewEntry(Guid userId, Guid jamId)
        {
            DateTime now = DateTime.Now;

            GameJamEntry newEntry = new GameJamEntry
            {
                CreateDate = now,
                JoinDate = now,
                UserId = userId,
                GameJamId = jamId
            };

            CheckTeamMembers(newEntry);

            return newEntry;
        }

        private static void CheckTeamMembers(Models.GameJamEntry obj)
        {
            if (obj.TeamMembers == null || !obj.TeamMembers.Any())
            {
                GameJamTeamMember meTeamMember = new Models.GameJamTeamMember
                {
                    UserId = obj.UserId,
                    TeamJoinDate = DateTime.Now,
                    IsSubmitter = true
                };

                obj.TeamMembers = new List<Models.GameJamTeamMember> { meTeamMember };
            }
        }
    }
}