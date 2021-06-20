using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class PlatformSetting : Entity
    {
        public PlatformSettingElement Element { get; set; }

        public string Value { get; set; }
    }
}