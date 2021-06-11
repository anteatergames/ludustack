using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumTopicReplyListVo : GenericPaginatedListVo<ForumPost>
    {
        public List<ForumPost> Replies { get { return Items; } set { Items = value; } }
    }
}