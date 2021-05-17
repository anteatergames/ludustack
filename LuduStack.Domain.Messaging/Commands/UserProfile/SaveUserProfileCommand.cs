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
    public class SaveUserProfileCommand : BaseUserCommand
    {
        public UserProfile UserProfile { get; }
        public bool HasCoverImage { get; }

        public SaveUserProfileCommand(Guid userId, UserProfile userProfile, bool hasCoverImage) : base(userId, userProfile.Id)
        {
            UserProfile = userProfile;
            HasCoverImage = hasCoverImage;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveUserProfileCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveUserProfileCommandHandler : CommandHandler, IRequestHandler<SaveUserProfileCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserProfileRepository userProfileRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveUserProfileCommandHandler(IUnitOfWork unitOfWork, IUserProfileRepository userProfileRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.userProfileRepository = userProfileRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveUserProfileCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.UserProfile.Type == 0)
            {
                request.UserProfile.Type = ProfileType.Personal;
            }

            if (request.UserProfile.Id == Guid.Empty)
            {
                request.UserProfile.HasCoverImage = false;
                request.UserProfile.Handler = request.UserProfile.Handler.ToLower();
                await userProfileRepository.Add(request.UserProfile);
            }
            else
            {
                request.UserProfile.HasCoverImage = request.HasCoverImage;
                request.UserProfile.Handler = request.UserProfile.Handler.ToLower();
                userProfileRepository.Update(request.UserProfile);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}