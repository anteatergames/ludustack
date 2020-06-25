using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class UserPreferences : Entity
    {
        public SupportedLanguage UiLanguage { get; set; }

        public string ContentLanguages { get; set; }

        public JobProfile JobProfile { get; set; }

        public StudyProfile StudyProfile { get; set; }
    }
}