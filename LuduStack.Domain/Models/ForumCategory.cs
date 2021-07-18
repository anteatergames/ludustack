using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class ForumCategory : Entity
    {
        public Guid GroupId { get; set; }

        public string Handler { get; set; }

        public string FeaturedImage { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public bool IsForGameJam { get; set; }
    }
}