namespace LuduStack.Domain.Messaging
{
    public class DeletePlatformSettingCommandValidation : BaseCommandValidation<DeletePlatformSettingCommand, Models.PlatformSetting>
    {
        public DeletePlatformSettingCommandValidation()
        {
            ValidateId();
        }
    }
}