using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.UserContent;
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
    public class UnlikeUserContentCommand : BaseUserCommand
    {
        public UnlikeUserContentCommand(Guid userId, Guid courseId) : base(userId, courseId)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new UnlikeUnlikeUserContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class UnlikeUserContentCommandHandler : CommandHandler, IRequestHandler<UnlikeUserContentCommand, CommandResult>
    {
        private readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserContentRepository repository;

        public UnlikeUserContentCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IUserContentRepository repository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        public async Task<CommandResult> Handle(UnlikeUserContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            UserContent userContent = await repository.GetById(request.Id);
            if (userContent is null)
            {
                AddError("The content doesn't exist.");
                return request.Result;
            }

            IEnumerable<UserContentLike> likes = await mediator.Query<GetLikesQuery, IEnumerable<UserContentLike>>(new GetLikesQuery(x => x.ContentId == request.Id));

            bool liked = likes.Any(x => x.UserId == request.UserId);
            if (!liked)
            {
                AddError("Content not liked");
                return request.Result;
            }
            else
            {
                await repository.RemoveLike(request.UserId, request.Id);

                request.Result.Validation = await Commit(unitOfWork);

                return request.Result;
            }
        }
    }
}