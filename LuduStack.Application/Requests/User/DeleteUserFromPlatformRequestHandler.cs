using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Requests.User
{
    public class DeleteUserFromPlatformRequestHandler : IRequestHandler<DeleteUserFromPlatformRequest, OperationResultVo>
    {
        private IProfileAppService profileAppService;
        private IUserContentAppService userContentAppService;

        public DeleteUserFromPlatformRequestHandler(IProfileAppService profileAppService, IUserContentAppService userContentAppService)
        {
            this.profileAppService = profileAppService;
            this.userContentAppService = userContentAppService;
        }

        public async Task<OperationResultVo> Handle(DeleteUserFromPlatformRequest request, CancellationToken cancellationToken)
        {
            bool canDelete = true;

            var comments = await userContentAppService.GetCommentsByUserId(request.CurrentUserId, request.UserId);

            if (comments.Success)
            {
                var castResult = comments as OperationResultListVo<CommentViewModel>;

                canDelete = !castResult.Value.Any();
            }

            if (canDelete)
            {
                var result = profileAppService.Remove(request.CurrentUserId, request.UserId);

                return result;
            }
            else
            {
                return new OperationResultVo(false, "Can't delete user");
            }
        }
    }
}
