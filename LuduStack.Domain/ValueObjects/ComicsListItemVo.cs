using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Domain.ValueObjects
{
    public class ComicsListItemVo
    {
        public Guid Id { get; set; }
        public int IssueNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FeaturedImage { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
