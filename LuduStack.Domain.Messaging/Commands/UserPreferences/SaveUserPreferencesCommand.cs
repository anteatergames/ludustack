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
    public class SaveUserPreferencesCommand : BaseUserCommand
    {
        public UserPreferences UserPreferences { get; }

        public SaveUserPreferencesCommand(Guid userId, UserPreferences userPreferences) : base(userId, userPreferences.Id)
        {
            UserPreferences = userPreferences;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveUserPreferencesCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveUserPreferencesCommandHandler : CommandHandler, IRequestHandler<SaveUserPreferencesCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserPreferencesRepository userPreferencesRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveUserPreferencesCommandHandler(IUnitOfWork unitOfWork, IUserPreferencesRepository userPreferencesRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.userPreferencesRepository = userPreferencesRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.UserPreferences.Id == Guid.Empty)
            {
                await userPreferencesRepository.Add(request.UserPreferences);
            }
            else
            {
                userPreferencesRepository.Update(request.UserPreferences);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}