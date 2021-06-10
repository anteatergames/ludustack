using LuduStack.Domain.Interfaces.Models;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostListVo : IPagination
    {
        public List<ForumPostListItemVo> Posts { get; set; }

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int TotalPageCount { get; set; }

        public string PaginationMessage { get; set; }

        public ForumPostListVo()
        {
            Posts = new List<ForumPostListItemVo>();
        }
    }
}