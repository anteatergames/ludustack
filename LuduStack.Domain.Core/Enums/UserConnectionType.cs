using LuduStack.Domain.Core.Attributes;

namespace LuduStack.Domain.Core.Enums
{
    public enum UserConnectionType
    {
        [UiInfo(Display = "WorkedTogether")]
        WorkedTogether = 1,

        [UiInfo(Display = "Mentor")]
        Mentor = 2,

        [UiInfo(Display = "Pupil")]
        Pupil = 3,

        [UiInfo(Display = "Fellow Developer")]
        FellowDeveloper = 4
    }
}