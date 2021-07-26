using System;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumCategoryConterDataVo
    {
        public Guid Id { get; set; }

        public Guid OriginalPostId { get; set; }

        public Guid UserId { get; set; }

        public Guid ForumCategoryId { get; set; }

        public bool IsOriginalPost { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }
    }
}