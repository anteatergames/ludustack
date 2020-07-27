using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Interfaces;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.ViewModels
{
    public abstract class BaseViewModel : IEntity, IBaseViewModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public bool CurrentUserIsOwner { get; set; }

        public bool CurrentUserLiked { get; set; }

        public bool CurrentUserFollowing { get; set; }

        public PermissionsVo Permissions { get; set; }

        public string CreateDateText
        {
            get
            {
                return CreateDate.ToString();
            }
        }

        protected BaseViewModel()
        {
            Permissions = new PermissionsVo();
            CreateDate = DateTime.Now;
        }
    }
}