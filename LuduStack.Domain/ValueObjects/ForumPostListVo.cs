using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostListVo
    {
        public List<ForumPostListItemVo> Posts { get; set; }

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int TotalPageCount { get; set; }

        public ForumPostListVo()
        {
            Posts = new List<ForumPostListItemVo>();
        }
    }
}
