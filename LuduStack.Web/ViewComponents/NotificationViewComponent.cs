using LuduStack.Application.Requests.Notification;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Extensions.ViewModelExtensions;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents
{
    public class NotificationViewComponent : BaseViewComponent
    {
        public NotificationViewComponent(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int qtd)
        {
            OperationResultVo serviceResult = await Mediator.Send(new ListUserNotificationsRequest(CurrentUserId, qtd));

            if (!serviceResult.Success)
            {
                ViewData["UnreadCount"] = 0;

                return View(new List<NotificationItemViewModel>());
            }
            else
            {
                OperationResultListVo<NotificationItemViewModel> castResult = serviceResult as OperationResultListVo<NotificationItemViewModel>;

                List<NotificationItemViewModel> model = castResult.Value.ToList();

                foreach (NotificationItemViewModel item in model)
                {
                    item.FormatDisplay(SharedLocalizer, Url);
                }

                ViewData["UnreadCount"] = model.Count(x => !x.IsRead);

                return await Task.Run(() => View(model));
            }
        }
    }
}