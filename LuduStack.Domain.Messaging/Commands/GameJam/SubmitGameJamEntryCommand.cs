using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SubmitGameJamEntryCommand : BaseUserCommand
    {
        public Guid GameId { get; set; }
        public string ExtraInformation { get; set; }

        public IEnumerable<Guid> TeamMembersIds { get; }

        public SubmitGameJamEntryCommand(Guid userId, Guid entryId, Guid gameId, string extraInformation, IEnumerable<Guid> teamMembersIds) : base(userId, entryId)
        {
            GameId = gameId;
            ExtraInformation = extraInformation;
            TeamMembersIds = teamMembersIds;
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
        protected readonly IGameJamEntryRepository entryRepository;
        protected readonly IGameJamDomainService gameJamDomainService;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SubmitGameJamEntryCommandHandler(IUnitOfWork unitOfWork, IGameJamEntryRepository entryRepository, IGameJamDomainService gameJamDomainService, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.entryRepository = entryRepository;
            this.gameJamDomainService = gameJamDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SubmitGameJamEntryCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            GameJamEntry existing = await entryRepository.GetById(request.Id);

            if (existing != null)
            {
                existing.GameId = request.GameId;
                existing.ExtraInformation = request.ExtraInformation;
                existing.SubmissionDate = DateTime.Now;

                gameJamDomainService.CheckTeamMembers(existing, request.TeamMembersIds);

                entryRepository.Update(existing);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamSubmit);

                result.Validation = await Commit(unitOfWork);
            }
            else
            {
                GameJamEntry entry = GenerateNewEntry(request.UserId, request.Id);
                entry.GameId = request.GameId;
                entry.ExtraInformation = request.ExtraInformation;
                entry.SubmissionDate = DateTime.Now;

                gameJamDomainService.CheckTeamMembers(entry, request.TeamMembersIds);

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