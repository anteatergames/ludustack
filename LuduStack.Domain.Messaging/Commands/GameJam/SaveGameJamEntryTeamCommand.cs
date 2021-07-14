using FluentValidation.Results;
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
        protected readonly IGameJamDomainService gameJamDomainService;

        public SaveGameJamEntryTeamCommandHandler(IUnitOfWork unitOfWork, IGameJamEntryRepository entryRepository, IGameJamDomainService gameJamDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.entryRepository = entryRepository;
            this.gameJamDomainService = gameJamDomainService;
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

            gameJamDomainService.CheckTeamMembers(existing, request.TeamMembersIds);

            entryRepository.Update(existing);

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}