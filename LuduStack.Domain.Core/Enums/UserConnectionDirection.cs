using LuduStack.Domain.Core.Attributes;

namespace LuduStack.Domain.Core.Enums
{
    public enum UserConnectionDirection
    {
        [UiInfo(Display = "Both Ways")]
        BothWays = 1,

        [UiInfo(Display = "From User")]
        FromUser = 2,

        [UiInfo(Display = "To User")]
        ToUser = 3
    }
}