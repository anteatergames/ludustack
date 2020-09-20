using LuduStack.Application.Requests.Notification;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Member.Controllers.Base;
using LuduStack.Web.Extensions.ViewModelExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Member.Controllers
{
    [Authorize]
    [Route("notification")]
    public class NotificationController : MemberBaseController
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }

        [Route("listmine")]
        public async Task<PartialViewResult> ListMine([FromServices] IMediator mediator)
        {
            List<NotificationItemViewModel> model;

            OperationResultVo serviceResult = await mediator.Send(new ListUserNotificationsRequest(CurrentUserId));

            if (serviceResult.Success)
            {
                OperationResultListVo<NotificationItemViewModel> castResult = serviceResult as OperationResultListVo<NotificationItemViewModel>;

                model = castResult.Value.ToList();

                foreach (NotificationItemViewModel item in model)
                {
                    item.FormatDisplay(SharedLocalizer, Url);
                }
            }
            else
            {
                model = new List<NotificationItemViewModel>();
            }

            ViewData["ListDescription"] = SharedLocalizer["Sent to me"].ToString();

            return PartialView("_ListNotifications", model);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromServices] IMediator mediator, Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await mediator.Send(new DeleteNotificationRequest(CurrentUserId, id));

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "notification", new { area = "member", msg = deleteResult.Message });
                        deleteResult.Message = null;

                        return Json(new OperationResultRedirectVo(deleteResult, url));
                    }

                    return Json(deleteResult);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }
    }
}