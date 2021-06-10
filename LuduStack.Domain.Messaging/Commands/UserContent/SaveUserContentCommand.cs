using FluentValidation.Results;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveUserContentCommand : BaseUserCommand
    {
        public UserContent UserContent { get; }

        public bool IsComplex { get; }

        public Poll Poll { get; }

        public SaveUserContentCommand(Guid userId, UserContent userContent) : base(userId, userContent.Id)
        {
            UserContent = userContent;
            IsComplex = userContent.UserContentType == UserContentType.ComicStrip;
        }

        public SaveUserContentCommand(Guid userId, UserContent userContent, Poll poll) : base(userId, userContent.Id)
        {
            UserContent = userContent;
            IsComplex = false;
            Poll = poll;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveUserContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveUserContentCommandHandler : CommandHandler, IRequestHandler<SaveUserContentCommand, CommandResult>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserContentRepository userContentRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveUserContentCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IUserContentRepository userContentRepository, IGamificationDomainService gamificationDomainService)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.userContentRepository = userContentRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveUserContentCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            string youtubePattern = @"(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+";

            request.UserContent.Content = Regex.Replace(request.UserContent.Content, youtubePattern, delegate (Match match)
            {
                string v = match.ToString();
                if (match.Index == 0 && string.IsNullOrWhiteSpace(request.UserContent.FeaturedImage))
                {
                    request.UserContent.FeaturedImage = v;
                }
                return v;
            });

            if (request.UserContent.Id == Guid.Empty)
            {
                request.UserContent.UserId = request.UserId;

                await userContentRepository.Add(request.UserContent);

                PlatformAction action = request.IsComplex ? PlatformAction.ComplexPost : PlatformAction.SimplePost;
                result.PointsEarned += gamificationDomainService.ProcessAction(request.UserId, action);
            }
            else
            {
                userContentRepository.Update(request.UserContent);
            }

            result.Validation = await Commit(unitOfWork);

            if (request.Poll != null)
            {
                request.Poll.UserId = request.UserId;
                request.Poll.UserContentId = request.UserContent.Id;

                CommandResult savePollResult = await mediator.SendCommand(new SavePollCommand(request.UserId, request.Poll));

                if (!savePollResult.Validation.IsValid)
                {
                    string savePollMessage = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    result.Validation.Errors.Add(new ValidationFailure("Poll", savePollMessage));
                }

                result.PointsEarned += savePollResult.PointsEarned;
            }

            if (request.UserContent.GameId.HasValue && request.UserContent.Media != null && request.UserContent.Media.Any())
            {
                CommandResult addImagesToGameResult = await mediator.SendCommand(new AddImagesToGameCommand(request.UserContent.GameId.Value, request.UserContent.Media));

                if (!addImagesToGameResult.Validation.IsValid)
                {
                    string savePollMessage = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    result.Validation.Errors.Add(new ValidationFailure("Media", savePollMessage));
                }
            }

            return result;
        }
    }
}