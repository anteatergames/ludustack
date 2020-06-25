using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.UserLike
{
    public class UserLikeViewModel : BaseViewModel
    {
        public Guid LikedId { get; set; }

        public LikeTargetType TargetType { get; set; }
    }
}