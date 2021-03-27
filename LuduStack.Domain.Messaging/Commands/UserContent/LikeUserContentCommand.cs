﻿using LuduStack.Domain.Interfaces;
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
    public class LikeUserContentCommand : BaseCommand
    {
        public Guid UserId { get; }

        public LikeUserContentCommand(Guid userId, Guid courseId) : base(courseId)
        {
            UserId = userId;
        }

        public override bool IsValid()
        {
            Result.Validation = new LikeUserContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class LikeUserContentCommandHandler : CommandHandler, IRequestHandler<LikeUserContentCommand, CommandResult>
    {
        private readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserContentRepository repository;

        public LikeUserContentCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IUserContentRepository repository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        public async Task<CommandResult> Handle(LikeUserContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return request.Result;

            var userContent = await repository.GetById(request.Id);
            if (userContent is null)
            {
                AddError("The content doesn't exists.");
                return request.Result;
            }

            var likes = await mediator.Query<GetLikesQuery, IEnumerable<UserContentLike>>(new GetLikesQuery(x => x.ContentId == request.Id));

            bool alreadyLiked = likes.Any(x => x.UserId == request.UserId);
            if (alreadyLiked)
            {
                AddError("Content already liked");
                return request.Result;
            }
            else
            {
                UserContentLike model = new UserContentLike
                {
                    ContentId = request.Id,
                    UserId = request.UserId
                };

                await repository.AddLike(model);

                request.Result.Validation = await Commit(unitOfWork);

                return request.Result;
            }
        }
    }
}