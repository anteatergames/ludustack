using System;

namespace LuduStack.Domain.Messaging
{
    public class SavePlatformSettingCommandValidation : BaseUserCommandValidation<SavePlatformSettingCommand, Guid>
    {
        public SavePlatformSettingCommandValidation()
        {
            ValidateUserId();
        }
    }
}