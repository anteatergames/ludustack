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
    public class SubmitGameJamEntryCommand : BaseUserCommand
    {
        public Guid GameId { get; set; }

        public SubmitGameJamEntryCommand(Guid userId, Guid jamId, Guid gameId) : base(userId, jamId)
        {
            GameId = gameId;
        }

        public override bool IsValid()
        {
            Result.Validation = new SubmitGameJamEntryCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SubmitGameJamEntryCommandHandler : CommandHandler, IRequestHandler<SubmitGameJamEntryCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamRepository gameJamRepository;
        protected readonly IGameJamEntryRepository entryRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SubmitGameJamEntryCommandHandler(IUnitOfWork unitOfWork, IGameJamRepository gameJamRepository, IGameJamEntryRepository entryRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.gameJamRepository = gameJamRepository;
            this.entryRepository = entryRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SubmitGameJamEntryCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            GameJamEntry entry;

            System.Collections.Generic.IEnumerable<GameJamEntry> existing = await entryRepository.GetByUserId(request.UserId);

            if (existing.Any())
            {
                entry = existing.FirstOrDefault();
                entry.GameId = request.GameId;
                entry.SubmissionDate = DateTime.Now;

                entryRepository.Update(entry);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamSubmit);

                result.Validation = await Commit(unitOfWork);
            }
            else
            {
                entry = GenerateNewEntry(request.UserId, request.Id);
                entry.GameId = request.GameId;
                entry.SubmissionDate = DateTime.Now;

                await entryRepository.Add(entry);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamJoin);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamSubmit);

                result.Validation = await Commit(unitOfWork);
            }

            result.PointsEarned = pointsEarned;

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

            return newEntry;
        }
    }
}