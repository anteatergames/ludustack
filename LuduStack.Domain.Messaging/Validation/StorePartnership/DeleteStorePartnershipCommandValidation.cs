namespace LuduStack.Domain.Messaging
{
    public class DeleteStorePartnershipCommandValidation : BaseCommandValidation<DeleteStorePartnershipCommand>
    {
        public DeleteStorePartnershipCommandValidation()
        {
            ValidateId();
        }
    }
}