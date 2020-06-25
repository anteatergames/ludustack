using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Extensions.ViewModelExtensions;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents
{
    public class NotificationViewComponent : BaseViewComponent
    {
        private readonly INotificationAppService _notificationAppService;

        public NotificationViewComponent(IHttpContextAccessor httpContextAccessor, INotificationAppService notificationAppService) : base(httpContextAccessor)
        {
            _notificationAppService = notificationAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int qtd)
        {
            if (qtd == 0)
            {
                qtd = 10;
            }

            OperationResultListVo<NotificationItemViewModel> result = _notificationAppService.GetByUserId(CurrentUserId, qtd);

            System.Collections.Generic.List<NotificationItemViewModel> model = result.Value.ToList();

            foreach (NotificationItemViewModel item in model)
            {
                item.Url = string.Format("{0}?notificationclicked={1}", item.Url, item.Id);
            }

            model.DefineVisuals();

            ViewData["UnreadCount"] = model.Count(x => !x.IsRead);

            return await Task.Run(() => View(model));
        }
    }
}