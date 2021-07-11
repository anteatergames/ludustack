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
    public class SubmitGameJamEntryCommand : BaseUserCommand
    {
        public Guid GameId { get; set; }

        public IEnumerable<Guid> TeamMembersIds { get; }


        public SubmitGameJamEntryCommand(Guid userId, Guid entryId, Guid gameId, IEnumerable<Guid> teamMembersIds) : base(userId, entryId)
        {
            GameId = gameId;
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
        protected readonly IGamificationDomainService gamificationDomainService;

        public SubmitGameJamEntryCommandHandler(IUnitOfWork unitOfWork, IGameJamEntryRepository entryRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.entryRepository = entryRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SubmitGameJamEntryCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }


            var existing = await entryRepository.GetById(request.Id);

            if (existing != null)
            {
                existing.GameId = request.GameId;
                existing.SubmissionDate = DateTime.Now;

                CheckTeamMembers(existing, request.TeamMembersIds);

                entryRepository.Update(existing);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamSubmit);

                result.Validation = await Commit(unitOfWork);
            }
            else
            {
                GameJamEntry entry = GenerateNewEntry(request.UserId, request.Id);
                entry.GameId = request.GameId;
                entry.SubmissionDate = DateTime.Now;

                CheckTeamMembers(entry, request.TeamMembersIds);

                await entryRepository.Add(entry);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamJoin);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.GameJamSubmit);

                result.Validation = await Commit(unitOfWork);
            }

            result.PointsEarned = pointsEarned;

            return result;
        }

        private static void CheckTeamMembers(GameJamEntry entry, IEnumerable<Guid> userIds)
        {
            if (userIds != null && userIds.Any())
            {
                if (entry.TeamMembers == null)
                {
                    entry.TeamMembers = new List<GameJamTeamMember>();
                }

                foreach (Guid newUserId in userIds)
                {
                    if (!entry.TeamMembers.Any(x => x.UserId == newUserId))
                    {
                        GameJamTeamMember newTeamMember = new GameJamTeamMember { UserId = newUserId, TeamJoinDate = DateTime.Now, IsSubmitter = entry.UserId == newUserId };

                        entry.TeamMembers.Add(newTeamMember);
                    }
                }

                IEnumerable<Guid> allMembers = entry.TeamMembers.Select(x => x.UserId);

                IEnumerable<Guid> membersToExclude = allMembers.Except(userIds);

                entry.TeamMembers = entry.TeamMembers.Where(x => userIds.Contains(x.UserId)).ToList();

                entry.IsTeam = entry.TeamMembers.Count > 1;
            }
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