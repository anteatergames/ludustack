using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostListVo : GenericPaginatedListVo<ForumPostListItemVo>
    {
        public List<ForumPostListItemVo> Posts { get => Items; set => Items = value; }
    }
}