using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
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
    public class SaveForumPostCommand : BaseCommand
    {
        public ForumPost ForumPost { get; }

        public SaveForumPostCommand(ForumPost gamificationLevel) : base(gamificationLevel.Id)
        {
            ForumPost = gamificationLevel;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveForumPostCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveForumPostCommandHandler : CommandHandler, IRequestHandler<SaveForumPostCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumPostRepository forumPostRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveForumPostCommandHandler(IUnitOfWork unitOfWork, IForumPostRepository gamificationLevelRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            forumPostRepository = gamificationLevelRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveForumPostCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            SetSlug(request.ForumPost);

            SetMissingLanguage(request.ForumPost);

            if (request.ForumPost.Id == Guid.Empty)
            {
                await forumPostRepository.Add(request.ForumPost);
            }
            else
            {
                forumPostRepository.Update(request.ForumPost);
            }

            result.Validation = await Commit(unitOfWork);

            if (request.ForumPost.OriginalPostId == Guid.Empty)
            {
                request.ForumPost.OriginalPostId = request.ForumPost.Id;
                request.ForumPost.IsOriginalPost = true;

                forumPostRepository.Update(request.ForumPost);

                result.Validation = await Commit(unitOfWork);
            }

            return result;
        }

        private static void SetMissingLanguage(ForumPost forumPost)
        {
            if (forumPost.Language == 0)
            {
                forumPost.Language = SupportedLanguage.English;
            }
        }

        private static void SetSlug(ForumPost forumPost)
        {
            if (string.IsNullOrWhiteSpace(forumPost.Slug))
            {
                forumPost.Slug = forumPost.Title.Slugify();
            }
        }
    }
}