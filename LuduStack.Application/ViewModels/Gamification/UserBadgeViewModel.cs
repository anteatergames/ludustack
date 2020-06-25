using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Gamification
{
    public class UserBadgeViewModel : BaseViewModel
    {
        public BadgeType Badge { get; set; }

        public string Description
        {
            get
            {
                return Badge.ToUiInfo().Description;
            }
        }

        public List<Guid> References { get; set; }

        public UserBadgeViewModel()
        {
            References = new List<Guid>();
        }
    }
}