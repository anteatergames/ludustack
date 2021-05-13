using FluentValidation;
using LuduStack.Domain.ValueObjects;

namespace LuduStack.Domain.Messaging
{
    public class AddParticipantCommandValidation : BaseCommandValidation<AddParticipantCommand, DomainOperationVo>
    {
        public AddParticipantCommandValidation()
        {
            ValidateId();
            ValidateEmail();
            ValidateUrlReferralBase();
        }

        protected void ValidateEmail()
        {
            RuleFor(c => c.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("No Email provided.");
        }

        protected void ValidateUrlReferralBase()
        {
            RuleFor(c => c.UrlReferralBase)
                .NotNull()
                .NotEmpty()
                .WithMessage("No UrlReferralBase provided.");
        }
    }
}