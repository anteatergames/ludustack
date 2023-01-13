using LuduStack.Domain.Models;
using System;

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