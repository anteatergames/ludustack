using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumTopicAnswerListVo : GenericPaginatedListVo<ForumPost>
    {
        public List<ForumPost> Answers { get { return Items; } set { Items = value; } }
    }
}