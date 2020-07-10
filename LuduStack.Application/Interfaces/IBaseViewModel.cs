using LuduStack.Domain.Core.Interfaces;
using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IBaseViewModel : IEntityBase
    {
        string CreateDateText { get; }
        bool CurrentUserFollowing { get; set; }
        bool CurrentUserIsOwner { get; set; }
        bool CurrentUserLiked { get; set; }
        PermissionsVo Permissions { get; set; }
    }
}