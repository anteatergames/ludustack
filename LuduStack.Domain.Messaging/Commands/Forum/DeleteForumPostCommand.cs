using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteForumPostCommand : BaseUserCommand<Models.ForumPost>
    {
        public DeleteForumPostCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteForumPostCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteForumPostCommandHandler : CommandHandler, IRequestHandler<DeleteForumPostCommand, CommandResult<Models.ForumPost>>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumPostRepository forumPostRepository;

        public DeleteForumPostCommandHandler(IUnitOfWork unitOfWork, IForumPostRepository forumPostRepository)
        {
            this.unitOfWork = unitOfWork;
            this.forumPostRepository = forumPostRepository;
        }

        public async Task<CommandResult<Models.ForumPost>> Handle(DeleteForumPostCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.ForumPost forumPost = await forumPostRepository.GetById(request.Id);

            if (forumPost is null)
            {
                AddError("The Forum Post doesn't exist.");
                return request.Result;
            }

            if (forumPost.IsOriginalPost)
            {
                System.Linq.IQueryable<Models.ForumPost> replies = forumPostRepository.Get(x => x.OriginalPostId == request.Id && !x.IsOriginalPost);

                foreach (Models.ForumPost reply in replies)
                {
                    forumPostRepository.Remove(reply.Id);
                }
            }

            // AddDomainEvent here

            forumPostRepository.Remove(forumPost.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            request.Result.Result = forumPost;

            return request.Result;
        }
    }
}