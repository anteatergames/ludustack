using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.Notification
{
    public class NotificationItemViewModel : BaseViewModel
    {
        #region Entity

        public NotificationType Type { get; set; }

        public Guid? OriginId { get; set; }

        public string OriginName { get; set; }

        public Guid? TargetId { get; set; }

        public string TargetName { get; set; }

        public bool IsRead { get; set; }

        #endregion Entity

        #region Extra

        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string IconColor { get; set; }

        #endregion Extra
    }
}