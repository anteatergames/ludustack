using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteUserProfileCommand : BaseUserCommand
    {
        public DeleteUserProfileCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteUserProfileCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteUserProfileCommandHandler : CommandHandler, IRequestHandler<DeleteUserProfileCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserProfileRepository userProfileRepository;

        public DeleteUserProfileCommandHandler(IUnitOfWork unitOfWork, IUserProfileRepository userProfileRepository)
        {
            this.unitOfWork = unitOfWork;
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<CommandResult> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.UserProfile userProfile = await userProfileRepository.GetById(request.Id);

            if (userProfile is null)
            {
                AddError("The User Profile doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            userProfileRepository.Remove(userProfile.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}