using LuduStack.Domain.Interfaces.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostListVo : GenericPaginatedListVo<ForumPostListItemVo>
    {
        public List<ForumPostListItemVo> Posts { get { return Items; } set { Items = value; } }
    }
}