using System;

namespace LuduStack.Domain.ValueObjects
{
    public class UserViewVo
    {
        public Guid? UserId { get; set; }

        public DateTime CreateDate { get; set; }
    }
}