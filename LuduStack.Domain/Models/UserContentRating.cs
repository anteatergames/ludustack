using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class UserContentRating : Entity
    {
        public decimal Score{ get; set; }
    }
}