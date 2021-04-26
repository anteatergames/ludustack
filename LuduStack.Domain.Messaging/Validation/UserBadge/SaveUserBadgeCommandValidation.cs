using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveUserBadgeCommandValidation : BaseUserCommandValidation<SaveUserBadgeCommand>
    {
        public SaveUserBadgeCommandValidation()
        {
            ValidateUserId();
            ValidateBadgeType();
            ValidateReference();
        }

        protected void ValidateBadgeType()
        {
            RuleFor(c => c.BadgeType)
                .NotNull()
                .WithMessage("No Badge Type to save.");
        }

        protected void ValidateReference()
        {
            RuleFor(c => c.ReferenceId)
                .NotNull()
                .WithMessage("No Reference to save.");
        }
    }
}