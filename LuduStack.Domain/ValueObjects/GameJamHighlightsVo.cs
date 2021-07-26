using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.ValueObjects
{
    public class GameJamHighlightsVo
    {
        public GameJamHighlight Highlight { get; set; }

        public bool Available { get; set; }
    }
}