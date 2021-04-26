using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class UserContentComment : Entity
    {
        public Guid? ParentCommentId { get; set; }

        public Guid UserContentId { get; set; }

        public string Text { get; set; }
    }
}