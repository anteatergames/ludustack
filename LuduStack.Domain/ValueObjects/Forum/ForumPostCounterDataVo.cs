using System;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostConterDataVo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsOriginalPost { get; set; }

        public Guid OriginalPostId { get; set; }

        public string Title { get; set; }

        public int Views { get; set; }
    }
}