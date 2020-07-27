using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class ShortUrl : Entity
    {
        public ShortUrlDestinationType DestinationType { get; set; }

        public string OriginalUrl { get; set; }

        public string Token { get; set; }

        public string NewUrl { get; set; }

        public int AccessCount { get; set; }
    }
}