using LuduStack.Domain.Models;

namespace LuduStack.Domain.Messaging
{
    public class TogglePlatformSettingCommandValidation : BaseUserCommandValidation<TogglePlatformSettingCommand, PlatformSetting>
    {
        public TogglePlatformSettingCommandValidation()
        {
            ValidateUserId();
        }
    }
}