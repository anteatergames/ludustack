using LuduStack.Domain.Core.Interfaces;
using LuduStack.Domain.Models;
using System;

namespace LuduStack.Domain.Specifications.Follow
{
    public class UserNotTheSameSpecification : ISpecification<UserFollow>
    {
        private Guid userBeingFollowed;

        public UserNotTheSameSpecification(Guid userBeingFollowed)
        {
            this.userBeingFollowed = userBeingFollowed;
        }

        public string ErrorMessage => "Can't follow the same user!";
        public bool IsSatisfied { get; private set; }

        public bool IsSatisfiedBy(UserFollow item)
        {
            IsSatisfied = item.UserId != userBeingFollowed;

            return IsSatisfied;
        }
    }
}