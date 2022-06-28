using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SyncStorePartnershipsCommandValidation : AbstractValidator<SyncStorePartnershipsCommand>
    {
        public SyncStorePartnershipsCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
        }
    }
}