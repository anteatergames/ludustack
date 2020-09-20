using LuduStack.Application.ViewModels.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LuduStack.Web.Extensions.ViewModelExtensions
{
    public static class NotificationItemViewModelExtensions
    {
        public static void FormatDisplay(this NotificationItemViewModel item, IStringLocalizer localizer, IUrlHelper url)
        {
            if (string.IsNullOrWhiteSpace(item.OriginName))
            {
                item.OriginName = localizer["someone"];
            }

            switch (item.Type)
            {
                case Domain.Core.Enums.NotificationType.ContentLike:
                    item.Text = localizer["{0} loves your the content you posted!", item.OriginName];
                    item.Url = url.Action("details", "content", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-heart";
                    item.IconColor = "text-red";
                    break;

                case Domain.Core.Enums.NotificationType.GameLike:
                    item.Text = localizer["{0} loves your game {1}", item.OriginName, item.TargetName];
                    item.Url = url.Action("details", "game", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-heart";
                    item.IconColor = "text-red";
                    break;

                case Domain.Core.Enums.NotificationType.ContentComment:
                    item.Text = localizer["{0} has commented on your post.", item.OriginName];
                    item.Url = url.Action("details", "content", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-comment";
                    item.IconColor = "text-black";
                    break;

                case Domain.Core.Enums.NotificationType.ConnectionRequest:
                    item.Text = localizer["{0} wants to connect with you.", item.OriginName];
                    item.Url = url.Action("Details", "Profile", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-user-plus";
                    item.IconColor = "text-maroon";
                    break;

                case Domain.Core.Enums.NotificationType.FollowYou:
                    item.Text = localizer["{0} is following you now!", item.OriginName];
                    item.Url = url.Action("Details", "Profile", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-eye";
                    item.IconColor = "text-blue";
                    break;

                case Domain.Core.Enums.NotificationType.FollowYourGame:
                    item.Text = localizer["{0} is following your game {1}", item.OriginName, item.TargetName];
                    item.Url = url.Action("details", "game", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-eye";
                    item.IconColor = "text-purple";
                    break;

                case Domain.Core.Enums.NotificationType.AchivementEarned:
                    item.Text = localizer["You earned an achievement!"];
                    item.Url = url.Action("index", "home", new { area = string.Empty });
                    item.Icon = "fas fa-trophy";
                    item.IconColor = "text-yellow";
                    break;

                case Domain.Core.Enums.NotificationType.LevelUp:
                    item.Text = localizer["You leveled up!"];
                    item.Url = url.Action("index", "home", new { area = string.Empty });
                    item.Icon = "fas fa-angle-double-up";
                    item.IconColor = "text-yellow";
                    break;

                case Domain.Core.Enums.NotificationType.ArticleAboutYourGame:
                    item.Text = localizer["{0} wrote an article about your game.", item.OriginName];
                    item.Url = url.Action("details", "game", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "far fa-newspaper";
                    item.IconColor = "text-black";
                    break;

                case Domain.Core.Enums.NotificationType.ContentPosted:
                    item.Text = localizer["{0} posted a new content!", item.OriginName];
                    item.Url = url.Action("details", "content", new { area = string.Empty, id = item.TargetId });
                    item.Icon = "fas fa-file";
                    item.IconColor = "text-light-blue";
                    break;

                case Domain.Core.Enums.NotificationType.TeamInvitation:
                    item.Text = localizer["{0} has invited you to join a team!", item.OriginName];
                    item.Url = url.Action("details", "team", new { area = string.Empty, teamId = item.TargetId });
                    item.Icon = "fas fa-users";
                    item.IconColor = "text-green";
                    break;

                case Domain.Core.Enums.NotificationType.ComicsLike:
                    item.Text = localizer["{0} loves your comic strip!", item.OriginName];
                    item.Url = url.Action("details", "comics", new { area = "member", id = item.TargetId });
                    item.Icon = "fas fa-heart";
                    item.IconColor = "text-red";
                    break;

                default:
                    item.Text = localizer["New Content!"];
                    item.Url = url.Action("index", "home", new { area = string.Empty });
                    item.Icon = "far fa-dot-circle";
                    item.IconColor = "text-black";
                    break;
            }

            item.Url = string.Format("{0}?notificationclicked={1}", item.Url, item.Id);
        }
    }
}