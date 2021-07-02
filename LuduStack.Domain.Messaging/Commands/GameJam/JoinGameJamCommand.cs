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

            var existing = await entryRepository.GetByUserId(request.UserId);

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
            var now = DateTime.Now;

            var newEntry = new GameJamEntry
            {
                CreateDate = now,
                JoinDate = now,
                UserId = userId,
                GameJamId = jamId
            };

            return newEntry;
        }
    }
}