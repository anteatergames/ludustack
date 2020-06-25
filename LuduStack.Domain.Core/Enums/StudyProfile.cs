using LuduStack.Domain.Core.Attributes;

namespace LuduStack.Domain.Core.Enums
{
    public enum StudyProfile
    {
        [UiInfo(Display = "Mentor")]
        Mentor = 1,

        [UiInfo(Display = "Student")]
        Student = 2
    }
}