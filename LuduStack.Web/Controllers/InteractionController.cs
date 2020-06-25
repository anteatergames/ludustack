using LuduStack.Application.Interfaces;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LuduStack.Web.Controllers
{
    [Route("interact")]
    public class InteractionController : SecureBaseController
    {
        private readonly IPollAppService pollAppService;

        public InteractionController(IPollAppService pollAppService)
        {
            this.pollAppService = pollAppService;
        }

        #region Poll

        [HttpPost]
        [Route("poll/vote")]
        public IActionResult PollVote(Guid pollOptionId)
        {
            OperationResultVo response = pollAppService.PollVote(CurrentUserId, pollOptionId);

            return Json(response);
        }

        #endregion Poll
    }
}