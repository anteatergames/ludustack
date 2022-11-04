using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveStorePartnershipCommandValidation : BaseCommandValidation<SaveStorePartnershipCommand>
    {
        public SaveStorePartnershipCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.StorePartnership)
                .NotNull()
                .WithMessage("No Store Partnership to save.");
        }
    }
}