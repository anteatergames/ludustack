using LuduStack.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Domain.Models
{
    public class ShortUrl : Entity
    {
        public string OriginalUrl { get; set; }

        public string Token { get; set; }

        public string NewUrl { get; set; }

        public int AccessCount { get; set; }
    }
}
