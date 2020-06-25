using LuduStack.Domain.Core.Enums;

namespace LuduStack.Application.ViewModels.Notification
{
    public class NotificationItemViewModel : BaseViewModel
    {
        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string IconColor { get; set; }

        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }
    }
}