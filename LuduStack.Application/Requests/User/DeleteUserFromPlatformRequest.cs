using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Requests.User
{
    public class DeleteUserFromPlatformRequest : Command<OperationResultVo>
    {
        public Guid CurrentUserId { get; }
        public Guid UserId { get; }

        public DeleteUserFromPlatformRequest(Guid currentUserId, Guid userId)
        {
            CurrentUserId = currentUserId;
            UserId = userId;
        }
    }

    public class DeleteUserFromPlatformRequestHandler : CommandHandler, IRequestHandler<DeleteUserFromPlatformRequest, CommandResult<OperationResultVo>>
    {
        private readonly IProfileAppService profileAppService;
        private readonly IUserContentAppService userContentAppService;

        public DeleteUserFromPlatformRequestHandler(IProfileAppService profileAppService, IUserContentAppService userContentAppService)
        {
            this.profileAppService = profileAppService;
            this.userContentAppService = userContentAppService;
        }

        public async Task<CommandResult<OperationResultVo>> Handle(DeleteUserFromPlatformRequest request, CancellationToken cancellationToken)
        {
            bool canDelete = true;

            OperationResultVo comments = await userContentAppService.GetCommentsByUserId(request.CurrentUserId, request.UserId);

            if (comments.Success)
            {
                OperationResultListVo<CommentViewModel> castResult = comments as OperationResultListVo<CommentViewModel>;

                canDelete = !castResult.Value.Any();
            }

            if (canDelete)
            {
                ViewModels.User.ProfileViewModel profile = await profileAppService.GetByUserId(request.UserId, ProfileType.Personal, false);

                if (profile == null)
                {
                    return new CommandResult<OperationResultVo>(new OperationResultVo(false, "Can't delete user"));
                }

                OperationResultVo resultVo = await profileAppService.Remove(request.CurrentUserId, profile.Id);

                return new CommandResult<OperationResultVo>(resultVo);
            }
            else
            {
                return new CommandResult<OperationResultVo>(new OperationResultVo(false, "Can't delete user"));
            }
        }
    }
}