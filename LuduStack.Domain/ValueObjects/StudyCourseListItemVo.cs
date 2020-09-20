using System;

namespace LuduStack.Domain.ValueObjects
{
    public class StudyCourseListItemVo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public bool OpenForApplication { get; set; }

        public int StudentCount { get; set; }

        public string FeaturedImage { get; set; }
    }
}