using LuduStack.Domain.Core.Enums;

namespace LuduStack.Application.ViewModels.PlatformSetting
{
    public class PlatformSettingViewModel : BaseViewModel
    {
        public PlatformSettingType Type { get; set; }

        public PlatformSettingGroup Group { get; set; }

        public PlatformSettingElement Element { get; set; }

        public string Value { get; set; }

        public string DefaultValue { get; set; }

        public bool Saved { get; set; }

        public bool IsDefault => Value.Equals(DefaultValue);
    }
}