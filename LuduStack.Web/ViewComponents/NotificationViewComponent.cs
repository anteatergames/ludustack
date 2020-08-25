using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Extensions.ViewModelExtensions;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

            List<NotificationItemViewModel> model = result.Value.ToList();

            foreach (NotificationItemViewModel item in model)
            {
                SetNotificationDisplayData(item);
            }

            ViewData["UnreadCount"] = model.Count(x => !x.IsRead);

            return await Task.Run(() => View(model));
        }

        private void SetNotificationDisplayData(NotificationItemViewModel item)
        {
            if (string.IsNullOrWhiteSpace(item.OriginName))
            {
                item.OriginName = SharedLocalizer["someone"];
            }

            switch (item.Type)
            {
                case Domain.Core.Enums.NotificationType.ContentLike:
                    item.Text = SharedLocalizer["{0} loves your the content you posted!", item.OriginName];
                    item.Url = Url.Action("details", "content", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-heart";
                    item.IconColor = "text-red";
                    break;
                case Domain.Core.Enums.NotificationType.GameLike:
                    item.Text = SharedLocalizer["{0} loves your game {1}", item.OriginName, item.TargetName];
                    item.Url = Url.Action("details", "game", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-heart";
                    item.IconColor = "text-red";
                    break;
                case Domain.Core.Enums.NotificationType.ContentComment:
                    item.Text = SharedLocalizer["{0} has commented on your post.", item.OriginName];
                    item.Url = Url.Action("details", "content", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-comment";
                    item.IconColor = "text-black";
                    break;
                case Domain.Core.Enums.NotificationType.ConnectionRequest:
                    item.Text = SharedLocalizer["{0} wants to connect with you.", item.OriginName];
                    item.Url = Url.Action("Details", "Profile", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-user-plus";
                    item.IconColor = "text-maroon";
                    break;
                case Domain.Core.Enums.NotificationType.FollowYou:
                    item.Text = SharedLocalizer["{0} is following you now!", item.OriginName];
                    item.Url = Url.Action("Details", "Profile", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-eye";
                    item.IconColor = "text-blue";
                    break;
                case Domain.Core.Enums.NotificationType.FollowYourGame:
                    item.Text = SharedLocalizer["{0} is following your game {1}", item.OriginName, item.TargetName];
                    item.Url = Url.Action("details", "game", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-eye";
                    item.IconColor = "text-purple";
                    break;
                case Domain.Core.Enums.NotificationType.AchivementEarned:
                    item.Text = SharedLocalizer["You earned an achievement!"];
                    item.Url = Url.Action("index", "home", new { area = string.Empty });
                    item.Icon = "fas fa-trophy";
                    item.IconColor = "text-yellow";
                    break;
                case Domain.Core.Enums.NotificationType.LevelUp:
                    item.Text = SharedLocalizer["You leveled up!"];
                    item.Url = Url.Action("index", "home", new { area = string.Empty });
                    item.Icon = "fas fa-angle-double-up";
                    item.IconColor = "text-yellow";
                    break;
                case Domain.Core.Enums.NotificationType.ArticleAboutYourGame:
                    item.Text = SharedLocalizer["{0} wrote an article about your game.", item.OriginName];
                    item.Url = Url.Action("details", "game", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "far fa-newspaper";
                    item.IconColor = "text-black";
                    break;
                case Domain.Core.Enums.NotificationType.ContentPosted:
                    item.Text = SharedLocalizer["{0} posted a new content!", item.OriginName];
                    item.Url = Url.Action("details", "content", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-file";
                    item.IconColor = "text-light-blue";
                    break;
                case Domain.Core.Enums.NotificationType.TeamInvitation:
                    item.Text = SharedLocalizer["{0} has invited you to join a team!", item.OriginName];
                    item.Url = Url.Action("details", "team", new { area = string.Empty, teamId = item.TargetId });
                    item.Icon = "fas fa-users";
                    item.IconColor = "text-green";
                    break;
                case Domain.Core.Enums.NotificationType.ComicsLike:
                    item.Text = SharedLocalizer["{0} loves your comic strip!", item.OriginName];
                    item.Url = Url.Action("details", "comics", new { area = "member", id = item.TargetId });
                    item.Icon = "fas fa-heart";
                    item.IconColor = "text-red";
                    break;
                default:
                    item.Text = SharedLocalizer["New Content!"];
                    item.Url = Url.Action("index", "home", new { area = string.Empty });
                    item.Icon = "far fa-dot-circle";
                    item.IconColor = "text-black";
                    break;
            }

            item.Url = string.Format("{0}?notificationclicked={1}", item.Url, item.Id);
        }
    }
}