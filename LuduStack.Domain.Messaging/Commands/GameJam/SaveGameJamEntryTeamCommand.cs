using FluentValidation.Results;
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
    public class SaveGameJamEntryTeamCommand : BaseCommand
    {
        public IEnumerable<Guid> TeamMembersIds { get; }


        public SaveGameJamEntryTeamCommand(Guid entryId, IEnumerable<Guid> teamMembersIds) : base(entryId)
        {
            TeamMembersIds = teamMembersIds;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveGameJamEntryTeamCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveGameJamEntryTeamCommandHandler : CommandHandler, IRequestHandler<SaveGameJamEntryTeamCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamEntryRepository entryRepository;

        public SaveGameJamEntryTeamCommandHandler(IUnitOfWork unitOfWork, IGameJamEntryRepository entryRepository)
        {
            this.unitOfWork = unitOfWork;
            this.entryRepository = entryRepository;
        }

        public async Task<CommandResult> Handle(SaveGameJamEntryTeamCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            GameJamEntry existing = await entryRepository.GetById(request.Id);

            if (existing == null)
            {
                result.Validation.Errors.Add(new ValidationFailure(string.Empty, "Entry not found!"));
                return result;
            }

            CheckTeamMembers(existing, request.TeamMembersIds);

            entryRepository.Update(existing);

            result.Validation = await Commit(unitOfWork);

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
                        GameJamTeamMember newTeamMember = new GameJamTeamMember { UserId = newUserId, TeamJoinDate = DateTime.Now };

                        entry.TeamMembers.Add(newTeamMember);
                    }
                }

                IEnumerable<Guid> allMembers = entry.TeamMembers.Select(x => x.UserId);

                IEnumerable<Guid> membersToExclude = allMembers.Except(userIds);

                entry.TeamMembers = entry.TeamMembers.Where(x => userIds.Contains(x.UserId)).ToList();

                entry.IsTeam = entry.TeamMembers.Count > 1;
            }
        }
    }
}