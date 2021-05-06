using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.ValueObjects
{
    public class UserContentIdAndTypeVo
    {
        public Guid Id { get; set; }

        public UserContentType Type { get; set; }
    }
}