using FluentValidation.Results;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Commands.Forum
{
    public class RegisterForumPostViewCommand : BaseCommand
    {
        public Guid? UserId { get; }

        public RegisterForumPostViewCommand(Guid forumPostId, Guid? userId) : base(forumPostId)
        {
            UserId = userId;
        }
    }

    public class RegisterForumPostViewCommandHandler : CommandHandler, IRequestHandler<RegisterForumPostViewCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumPostRepository forumPostRepository;

        public RegisterForumPostViewCommandHandler(IUnitOfWork unitOfWork, IForumPostRepository forumPostRepository)
        {
            this.unitOfWork = unitOfWork;
            this.forumPostRepository = forumPostRepository;
        }

        public async Task<CommandResult> Handle(RegisterForumPostViewCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            bool canRegisterView = true;

            Models.ForumPost forumPost = await forumPostRepository.GetById(request.Id);

            if (forumPost is null)
            {
                AddError("The Forum Post doesn't exist.");
                return request.Result;
            }

            if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
            {
                canRegisterView = forumPostRepository.CanRegisterViewForUser(forumPost.Id, request.UserId.Value);
            }

            // AddDomainEvent here

            if (canRegisterView)
            {
                await forumPostRepository.RegisterView(forumPost.Id, request.UserId);

                ValidationResult validation = await Commit(unitOfWork);

                request.Result.Validation = validation;
            }

            return request.Result;
        }
    }
}